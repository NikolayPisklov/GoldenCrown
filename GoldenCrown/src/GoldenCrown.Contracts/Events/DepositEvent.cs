namespace GoldenCrown.Contracts.Events
{
    public record DepositEvent(int UserId, decimal Amount, string Currency);
}
