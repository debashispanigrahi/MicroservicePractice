using GrpcFileService.Repo;
using GrpcFileService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddGrpc();
// Add gRPC server reflection for tools like grpcurl (enabled in Development)
builder.Services.AddGrpcReflection();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Enable the reflection endpoint in development so clients can discover services
    app.MapGrpcReflectionService();
}

app.UseHttpsRedirection();

app.MapGrpcService<FileService>();

app.MapGet("/", () => "This server contains a gRPC file upload service.");

app.Run();
