namespace TransactionFormatter
{
    using System;
    using System.Globalization;
    using System.IO;
    using CsvHelper;
    using Core;

    public class CsvSink : ITransactionSink, IDisposable
    {
        private readonly CsvWriter _csv;

        public CsvSink(TextWriter writer)
        {
            _csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        }

        public void Sink(Transaction transaction)
        {
            _csv.WriteRecord(transaction);
            _csv.NextRecord();
        }

        public void Dispose()
        {
            _csv.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}