using GoldenCrown.Application.Abstractions;
using RabbitMQ.Client;
using System.Text.Json;

namespace GoldenCrown.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly IRabbitMqConnectionManager _connectionManager;

        public RabbitMqPublisher(IRabbitMqConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task PublishAsync(Guid messageId, string message, string messageType, CancellationToken cancellationToken)
        {
            await using var channel = await _connectionManager.CreateChannelAsync(cancellationToken);
            var body = JsonSerializer.SerializeToUtf8Bytes(message);
            var props = new BasicProperties
            {
                MessageId = messageId.ToString(),
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };
            var routingKey = messageType;
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: routingKey,
                mandatory: true,
                basicProperties: props,
                body: body,
                cancellationToken: cancellationToken);
        }
    }
}
