namespace GoldenCrownApi.Dtos.Account
{
    public class BalanceResponse
    {
        public decimal Balance { get; set; }
        public string AccountCurrency { get; set; } = null!;
    }
}
