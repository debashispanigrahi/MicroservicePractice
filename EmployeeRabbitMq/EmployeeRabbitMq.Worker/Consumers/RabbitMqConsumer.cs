using System.Text;
using System.Text.Json;
using EmployeeRabbitMq.Contracts.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmployeeRabbitMq.Worker.Consumers;

public class RabbitMqConsumer(IConfiguration configuration)
{
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        var host = configuration["RabbitMq:Host"]!;
        var port = int.Parse(configuration["RabbitMq:Port"]!);
        var username = configuration["RabbitMq:Username"]!;
        var password = configuration["RabbitMq:Password"]!;

        var exchange = configuration["RabbitMq:Exchange"]!;
        var queue = configuration["RabbitMq:Queue"]!;
        var routingKey = configuration["RabbitMq:RoutingKey"]!;

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = username,
            Password = password,
            ClientProvidedName = "EmployeeRabbitMq.Worker"
        };

        _connection = await factory.CreateConnectionAsync(
            cancellationToken);

        _channel = await _connection.CreateChannelAsync(
            cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange,
            ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue,
            exchange,
            routingKey,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();

            var json = Encoding.UTF8.GetString(body);

            var employee =
                JsonSerializer.Deserialize<EmployeeCreatedMessage>(json);

            Console.WriteLine("=================================");
            Console.WriteLine("Employee message received");
            Console.WriteLine($"EmployeeId: {employee?.EmployeeId}");
            Console.WriteLine($"Name:       {employee?.Name}");
            Console.WriteLine($"Email:      {employee?.Email}");
            Console.WriteLine($"CreatedAt:  {employee?.CreatedAt}");
            Console.WriteLine("=================================");

            await _channel.BasicAckAsync(
                eventArgs.DeliveryTag,
                multiple: false);
        };

        await _channel.BasicConsumeAsync(
            queue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        await Task.Delay(
            Timeout.Infinite,
            cancellationToken);
    }
}