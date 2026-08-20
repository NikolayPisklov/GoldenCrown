namespace GoldenCrown.Application.Abstractions
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken);
    }
}
