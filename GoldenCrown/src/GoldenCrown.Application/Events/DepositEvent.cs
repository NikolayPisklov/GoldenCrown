namespace GoldenCrown.Application.Events
{
    public record DepositEvent(int UserId, decimal Amount, string Currency);
}
