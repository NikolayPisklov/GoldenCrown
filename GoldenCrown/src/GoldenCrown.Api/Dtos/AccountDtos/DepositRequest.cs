namespace GoldenCrown.Api.Dtos.AccountDtos
{
    public class DepositRequest
    {
        public decimal Amount { get; set; }
        public int CurrencyToId { get; set; }
        public int CurrencyFromId { get; set; }
    }
}
