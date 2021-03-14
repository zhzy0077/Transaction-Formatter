namespace TransactionFormatter
{
    using System;
    using System.Collections.Generic;

    public interface ITransactionProvider
    {
        public IEnumerable<Transaction> Provides(string path);
    }

    public struct Transaction
    {
        public DateTime TransactionDate { get; set; }

        public string Description { get; set; }

        public string FromAccount { get; set; }

        public string ToAccount { get; set; }

        public decimal Amount { get; set; }
        
        public string ID { get; set; }
    }

    public interface ITransaction
    {
        public Transaction ToTransaction();
    }
}