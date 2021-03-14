namespace TransactionFormatter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using CsvHelper.Configuration;
    using CsvHelper.Configuration.Attributes;

    public class CmbChinaProvider : ITransactionProvider
    {
        public IEnumerable<Transaction> Provides(string path)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = header => header.Header.Trim(),
                TrimOptions = TrimOptions.Trim,
            };
            
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<CmbChinaClassMap>();
            var records = csv.GetRecords<CmbChina>();

            return records.Select(record => record.ToTransaction());
        }
    }
    
    public struct CmbChina : ITransaction
    {
        [Name("交易日期")] public DateTime TransactionDate { get; set; }

        [Name("记账日期")] public DateTime? BillingDate { get; set; }

        [Name("交易摘要")] public String Description { get; set; }

        [Name("交易地点")] public String Location { get; set; }

        [Name("卡号末四位")] public string Card { get; set; }

        [Name("人民币金额")] public decimal CnyAmount { get; set; }

        [Name("交易地金额")] public decimal LocalAmount { get; set; }

        public Transaction ToTransaction()
        {
            var account = AccountDetector.ResolveAccount(Card.ToString());
            var descAcc = AccountDetector.ResolveDescription(Description);
            // Expense
            if (CnyAmount > 0)
            {
                return new Transaction()
                {
                    Description = Description,
                    FromAccount = account,
                    ToAccount = descAcc,
                    Amount = CnyAmount,
                    TransactionDate = TransactionDate,
                };
            }
            else
            {
                return new Transaction()
                {
                    Description = Description,
                    FromAccount = descAcc,
                    ToAccount = account,
                    Amount = CnyAmount,
                    TransactionDate = TransactionDate,
                };
            }
        }
    }

    internal sealed class CmbChinaClassMap : ClassMap<CmbChina>
    {
        public CmbChinaClassMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.BillingDate).Convert(row =>
            {
                if (DateTime.TryParse(row.Row["记账日期"], out var date))
                {
                    return date;
                }

                return null;
            });
            Map(m => m.CnyAmount).Convert(row => decimal.Parse(row.Row["人民币金额"].Trim().Substring(1)));
        }
    }
}