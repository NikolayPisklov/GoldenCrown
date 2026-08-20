namespace GoldenCrown.Api.Dtos.AccountDtos
{
    public class DepositRequest
    {
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
    }
}
