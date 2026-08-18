namespace GoldenCrown.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int SenderAccountId { get; set; }
        public int ReceiverAccountId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }

        public Account ReceiverAccount { get; set; } = null!;
        public Account SenderAccount { get; set; } = null!;
    }
}
