using WebSocketEmployeeApi.Services;
using WebSocketEmployeeApi.WebSockets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<EmployeeWebSocketHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseWebSockets();
app.Map("/ws/employees", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        await context.Response.WriteAsync(
            "WebSocket connection required.");

        return;
    }

    var webSocket = await context.WebSockets.AcceptWebSocketAsync();

    var handler = context.RequestServices
        .GetRequiredService<EmployeeWebSocketHandler>();

    await handler.HandleAsync(webSocket);
});

app.Run();