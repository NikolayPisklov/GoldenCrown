namespace GoldenCrown.Application.Dtos
{
    public class TransactionInfo
    {
        public bool IsSender { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string AccountCurrency { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
