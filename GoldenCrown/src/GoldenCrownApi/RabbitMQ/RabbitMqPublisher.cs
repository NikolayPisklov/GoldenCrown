using GoldenCrown.Api.Common;
using GoldenCrown.Application.Abstractions;
using GoldenCrown.Application.Events;
using RabbitMQ.Client;
using System.Text.Json;

namespace GoldenCrown.Api.RabbitMQ
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly IRabbitMqConnectionManager _connectionManager;

        public RabbitMqPublisher(IRabbitMqConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
        {
            await using var channel = await _connectionManager.CreateChannelAsync(cancellationToken);
            var body = JsonSerializer.SerializeToUtf8Bytes(message);
            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };
            var routingKey = GetRoutingKey(message!);
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: routingKey,
                mandatory: true,
                basicProperties: props,
                body: body,
                cancellationToken: cancellationToken);
        }

        private static string GetRoutingKey(object message) => message switch
        {
            TransferEvent => RoutingKeys.Transaction.TransactionSend,
            DepositEvent => RoutingKeys.Transaction.TransactionDeposit,
            _ => throw new InvalidOperationException($"Не задан routing key для {message.GetType().Name}.")
        };
    }
}
