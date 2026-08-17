namespace GoldenCrownApi.Common
{
    public interface IRoutedMessage
    {
        string Exchange { get; }
        string RoutingKey { get; }
    }

    public record TransactionEvent(int SenderId, int RecieverId, decimal Amount, string Currency) : IRoutedMessage
    {
        public string Exchange => "";

        public string RoutingKey => RoutingKeys.Transaction.TransactionSend;
    }
    public record TransactionDepositEvent(int UserId, decimal Amount, string Currency) : IRoutedMessage
    {
        public string Exchange => "";

        public string RoutingKey => RoutingKeys.Transaction.TransactionDeposit;
    }
}
