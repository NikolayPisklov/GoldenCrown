namespace GoldenCrown.Contracts.Events
{
    public record TransferEvent(
        int TransactionId,
        int SenderId,
        int ReceiverId,
        decimal Amount,
        string CurrencyFrom,
        decimal ConvertedAmount,
        string CurrencyTo,
        decimal Rate,
        DateTime Date);
}
