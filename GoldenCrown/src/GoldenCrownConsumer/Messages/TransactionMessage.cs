namespace GoldenCrownConsumer.Messages
{
    internal record TransactionMessage(int SenderId, int RecieverId, decimal Amount, string Currency);
}
