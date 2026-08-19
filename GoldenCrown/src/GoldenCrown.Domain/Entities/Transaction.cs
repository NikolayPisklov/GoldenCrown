namespace GoldenCrown.Domain.Entities
{
    public class Transaction
    {
        private Transaction() { }

        public static Transaction CreateTransfer(Account sender, Account receiver, decimal amount) => new()
        {
            SenderAccountId = sender.Id,
            ReceiverAccountId = receiver.Id,
            Date = DateTime.UtcNow,
            Amount = amount
        };
        public static Transaction CreateDeposit(Account account, decimal amount) => new()
        {
            SenderAccountId = account.Id,
            ReceiverAccountId = account.Id,
            Date = DateTime.UtcNow,
            Amount = amount
        };

        public int Id { get; private set; }
        public int SenderAccountId { get; private set; }
        public int ReceiverAccountId { get; private set; }
        public DateTime Date { get; private set; }
        public decimal Amount { get; private set; }
    }
}
