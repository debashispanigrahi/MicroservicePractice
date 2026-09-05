using EmployeeRabbitMq.Worker;
using EmployeeRabbitMq.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<RabbitMqConsumer>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
