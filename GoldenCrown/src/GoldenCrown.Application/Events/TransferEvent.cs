namespace GoldenCrown.Application.Events
{
    public record TransferEvent(int SenderId, int ReceiverId, decimal Amount, string Currency);
}
