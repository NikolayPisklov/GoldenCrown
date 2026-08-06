namespace GoldenCrown.Dtos.Account
{
    public class TransferRequest
    {
        public string ReceiverLogin { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
