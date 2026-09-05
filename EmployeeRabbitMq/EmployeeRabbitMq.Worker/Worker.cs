using EmployeeRabbitMq.Worker.Consumers;

namespace EmployeeRabbitMq.Worker;

public class Worker(
    RabbitMqConsumer rabbitMqConsumer,
    ILogger<Worker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Employee RabbitMQ Worker starting...");

        await rabbitMqConsumer.StartAsync(stoppingToken);
    }
}
