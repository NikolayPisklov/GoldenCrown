using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using GoldenCrownConsumer.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: "transaction.deposit",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

await channel.QueueDeclareAsync(
    queue: "transaction.send",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var json = Encoding.UTF8.GetString(body);
    switch (ea.RoutingKey)
    {
        case "transaction.deposit":
            var depositMessage = JsonSerializer.Deserialize<DepositMessage>(json)!;
            Console.WriteLine($"Message received.\nMessage type: deposit\n\nDeposit details: - User ID: {depositMessage.UserId}\n - Amount: {depositMessage.Amount}\n- Currency: {depositMessage.Currency}");
            break;
        case "transaction.send":
            var transactionMessage = JsonSerializer.Deserialize<TransactionMessage>(json)!;
            Console.WriteLine($"Message received.\nMessage type: deposit\n\nDeposit details:\n- Sender ID: {transactionMessage.SenderId}\n- Reviever ID: {transactionMessage.RecieverId}\n- Amount: {transactionMessage.Amount}\n- Currency: {transactionMessage.Currency}");
            break;
    }
};

await channel.BasicConsumeAsync(
    queue: "transaction.deposit",
    autoAck: true,
    consumer: consumer);
await channel.BasicConsumeAsync(
    queue: "transaction.send",
    autoAck: true,
    consumer: consumer);

Console.ReadLine();