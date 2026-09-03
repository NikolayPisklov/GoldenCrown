namespace GoldenCrown.Application.Abstractions
{
    public interface IMessagePublisher
    {
        Task PublishAsync(Guid messageId, string message, string messageType, CancellationToken cancellationToken);
    }
}
