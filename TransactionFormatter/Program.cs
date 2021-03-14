namespace TransactionFormatter
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using CsvHelper.Configuration;

    static class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Users\zhizhen\code\TransactionFormatter\TransactionFormatter\sodexo.json";
            var provider = new SodexoProvider();
            
            var transactionBeginsAt = new DateTime(2021, 3, 8, 0, 0, 0);

            using var writer = new StreamWriter("out.csv");
            using var sink = new CsvSink(writer);
            var transactions = provider.Provides(path);
            transactions
                .OrderBy(trans => trans.TransactionDate)
                .Where(trans => trans.TransactionDate > transactionBeginsAt)
                .ToList()
                .ForEach(record => sink.Sink(record));
        }
    }
}