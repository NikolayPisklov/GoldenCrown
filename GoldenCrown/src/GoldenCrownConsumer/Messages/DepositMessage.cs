namespace GoldenCrownConsumer.Messages
{
    internal record DepositMessage(int UserId, decimal Amount, string Currency);
}
