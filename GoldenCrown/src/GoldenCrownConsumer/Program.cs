using GoldenCrown.Contracts.Events;
using GoldenCrownConsumer;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using static GoldenCrown.Contracts.RoutingKeys;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var rabbitSettings = configuration.GetSection("RabbitMq").Get<RabbitMqConfig>()
    ?? throw new InvalidOperationException("Секция RabbitMq не найдена в конфигурации.");

var factory = new ConnectionFactory
{
    HostName = rabbitSettings.HostName,
    Port = rabbitSettings.Port,
    UserName = rabbitSettings.UserName,
    Password = rabbitSettings.Password,
    VirtualHost = rabbitSettings.VirtualHost
};
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: Transaction.TransactionDeposit,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

await channel.QueueDeclareAsync(
    queue: Transaction.TransactionSend,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false);

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    var json = Encoding.UTF8.GetString(ea.Body.ToArray());

    try
    {
        switch (ea.RoutingKey)
        {
            case Transaction.TransactionDeposit:
                var depositEvent = JsonSerializer.Deserialize<DepositEvent>(json)
                    ?? throw new JsonException("Тело сообщения пустое.");
                Console.WriteLine($"Message received.\nMessage type: deposit\nDeposit details:\n- User ID: {depositEvent.UserId}\n- Amount: {depositEvent.Amount}\n- Currency: {depositEvent.Currency}");
                break;

            case Transaction.TransactionSend:
                var transferEvent = JsonSerializer.Deserialize<TransferEvent>(json)
                    ?? throw new JsonException("Тело сообщения пустое.");
                Console.WriteLine($"Message received.\nMessage type: transfer\nTransaction details:\n- Sender ID: {transferEvent.SenderId}\n- Receiver ID: {transferEvent.ReceiverId}\n- Amount: {transferEvent.Amount}\n- Currency: {transferEvent.Currency}");
                break;

            default:
                Console.WriteLine($"Unknown routing key: {ea.RoutingKey}. Message discarded.");
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                return;
        }

        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
    }
    catch (JsonException)
    {
        Console.WriteLine("Invalid message format. Message discarded.");
        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
    }
};

await channel.BasicConsumeAsync(
    queue: Transaction.TransactionDeposit,
    autoAck: false,
    consumer: consumer);
await channel.BasicConsumeAsync(
    queue: Transaction.TransactionSend,
    autoAck: false,
    consumer: consumer);

Console.WriteLine("Consumer started. Press Ctrl+C to exit.");

var stopSignal = new TaskCompletionSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    stopSignal.TrySetResult();
};
AppDomain.CurrentDomain.ProcessExit += (_, _) => stopSignal.TrySetResult();

await stopSignal.Task;
