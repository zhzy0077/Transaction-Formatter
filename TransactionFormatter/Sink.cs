namespace Core
{
    using TransactionFormatter;

    public interface ITransactionSink
    {
        public void Sink(Transaction transaction);
    }
}