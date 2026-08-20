namespace GoldenCrown.Api.Dtos.AccountDtos
{
    public class TransferRequest
    {
        public string ReceiverLogin { get; set; } = null!;
        public int CurrencyId { get; set; }
        public decimal Amount { get; set; }
    }
}
