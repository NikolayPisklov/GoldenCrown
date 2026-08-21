namespace GoldenCrown.Contracts.Events
{
    public record TransferEvent(int SenderId, int ReceiverId, decimal Amount, string Currency);
}
