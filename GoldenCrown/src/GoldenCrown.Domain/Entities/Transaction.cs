using GoldenCrown.Domain.Common;

namespace GoldenCrown.Domain.Entities
{
    public class Transaction
    {
        private Transaction() { }

        public static Transaction CreateTransfer(Account sender, Account receiver, decimal amount, decimal convertedAmount, decimal rate, string currencyFrom, string currencyTo) => new()
        {
            SenderAccountId = sender.Id,
            ReceiverAccountId = receiver.Id,
            Date = DateTime.UtcNow,
            Amount = amount,
            Rate = rate,
            ConvertedAmount = convertedAmount,
            CurrencyFrom = currencyFrom,
            CurrencyTo = currencyTo
        };
        public static Transaction CreateDeposit(Account account, decimal amount, decimal rate, string currencyFrom, string currencyTo, decimal convertedAmount) => new()
        {
            SenderAccountId = account.Id,
            ReceiverAccountId = account.Id,
            Date = DateTime.UtcNow,
            Amount = amount,
            Rate = rate,
            ConvertedAmount = convertedAmount,
            CurrencyFrom = currencyFrom,
            CurrencyTo = currencyTo
        };

        public int Id { get; private set; }
        public int SenderAccountId { get; private set; }
        public int ReceiverAccountId { get; private set; }
        public DateTime Date { get; private set; }
        public decimal Amount { get; private set; }
        public decimal ConvertedAmount { get; private set; }
        public decimal Rate { get; private set; }
        public string CurrencyFrom { get; private set; } = null!;
        public string CurrencyTo { get; private set; } = null!;
    }
}
