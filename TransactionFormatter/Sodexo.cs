namespace TransactionFormatter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Newtonsoft.Json;

    public class SodexoProvider : ITransactionProvider
    {
        public IEnumerable<Transaction> Provides(string path)
        {
            using var content = new StreamReader(path);
            using var reader = new JsonTextReader(content);
            var transactions = new JsonSerializer().Deserialize<dynamic>(reader);

            foreach (var trans in transactions.transactionData)
            {
                var datetime = DateTime.ParseExact((string)trans.TransactionDate + trans.TransactionTime, "yyyyMMddHH:mm:ss", CultureInfo.InvariantCulture);
                var desc = AccountDetector.ResolveDescription((string)trans.merchantName);
                if (trans.TransType == 1)
                {
                    yield return new Transaction()
                    {
                        TransactionDate = datetime,
                        Amount = trans.amount,
                        FromAccount = "sodexo",
                        ToAccount = desc,
                        Description = trans.merchantName,
                        ID = trans.transactionCode,
                    };
                }
                else
                {
                    yield return new Transaction()
                    {
                        TransactionDate = datetime,
                        Amount = trans.amount,
                        FromAccount = desc,
                        ToAccount = "sodexo",
                        Description = trans.merchantName,
                        ID = trans.transactionCode,
                    };
                }
            }
        }
    }
}