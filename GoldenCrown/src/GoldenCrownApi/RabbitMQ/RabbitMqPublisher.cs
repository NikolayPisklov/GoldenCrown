using GoldenCrownApi.Common;
using RabbitMQ.Client;
using System.Text.Json;

namespace GoldenCrownApi.RabbitMQ
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : IRoutedMessage;
    }
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly IRabbitMqConnectionManager _connectionManager;

        public RabbitMqPublisher(IRabbitMqConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : IRoutedMessage
        {
            await using var channel = await _connectionManager.CreateChannelAsync(cancellationToken);
            var body = JsonSerializer.SerializeToUtf8Bytes(message);
            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };
            await channel.BasicPublishAsync(
                exchange: message.Exchange,
                routingKey: message.RoutingKey,
                mandatory: true,
                basicProperties: props,
                body: body,
                cancellationToken: cancellationToken);
        }
    }
}
