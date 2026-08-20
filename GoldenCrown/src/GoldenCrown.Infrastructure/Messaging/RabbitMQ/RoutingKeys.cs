namespace GoldenCrown.Infrastructure.Messaging.RabbitMQ
{
    public static class RoutingKeys
    {
        public static class Transaction
        {
            public const string TransactionSend = "transaction.send";
            public const string TransactionDeposit = "transaction.deposit";
        }
    }
}
