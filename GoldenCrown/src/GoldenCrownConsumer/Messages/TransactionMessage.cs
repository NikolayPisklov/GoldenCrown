namespace GoldenCrownConsumer.Messages
{
    internal record TransactionMessage(int SenderId, int ReceiverId, decimal Amount, string Currency);
}
