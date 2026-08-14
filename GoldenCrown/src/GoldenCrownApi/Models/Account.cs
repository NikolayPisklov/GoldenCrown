namespace GoldenCrown.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CurrencyId { get; set; }
        public decimal Balance { get; set; } = 0;

        public Currency Currency { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();
        public ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
    }
}
