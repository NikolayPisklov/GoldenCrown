namespace GoldenCrown.Contracts.Events
{
    public record DepositEvent(
        int TransactionId,
        int UserId,
        decimal Amount,
        string CurrencyFrom,
        decimal ConvertedAmount,
        string CurrencyTo,
        decimal Rate,
        DateTime Date);
}
