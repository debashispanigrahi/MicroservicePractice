using System.Text;
using System.Text.Json;
using EmployeeRabbitMq.Contracts.Models;
using RabbitMQ.Client;

namespace EmployeeRabbitMq.Api.Services;

public class RabbitMqProducer(IConfiguration configuration) : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    private async Task EnsureConnectionAsync()
    {
        if (_connection is not null && _channel is not null)
            return;

        var host = configuration["RabbitMq:Host"]!;
        var port = int.Parse(configuration["RabbitMq:Port"]!);
        var username = configuration["RabbitMq:Username"]!;
        var password = configuration["RabbitMq:Password"]!;

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = username,
            Password = password,
            ClientProvidedName = "EmployeeRabbitMq.Api"
        };

        _connection = await factory.CreateConnectionAsync();

        _channel = await _connection.CreateChannelAsync();
    }

    public async Task PublishEmployeeCreatedAsync(
        EmployeeCreatedMessage message)
    {
        await EnsureConnectionAsync();

        var exchange = configuration["RabbitMq:Exchange"]!;
        var queue = configuration["RabbitMq:Queue"]!;
        var routingKey = configuration["RabbitMq:RoutingKey"]!;

        await _channel!.ExchangeDeclareAsync(
            exchange,
            ExchangeType.Direct,
            durable: true);

        await _channel.QueueDeclareAsync(
            queue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await _channel.QueueBindAsync(
            queue,
            exchange,
            routingKey);

        var json = JsonSerializer.Serialize(message);

        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await _channel.BasicPublishAsync(
            exchange,
            routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();

        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}