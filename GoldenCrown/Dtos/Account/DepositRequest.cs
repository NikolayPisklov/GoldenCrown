namespace GoldenCrown.Dtos.Account
{
    public class DepositRequest
    {
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
    }
}
