namespace GoldenCrown.Api.Dtos.AccountDtos
{
    public class TransferRequest
    {
        public string ReceiverLogin { get; set; } = null!;
        public int FromCurrencyId { get; set; }
        public int ToCurrencyId { get; set; }
        public decimal Amount { get; set; }
    }
}
