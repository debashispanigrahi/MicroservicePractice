using System.Text;
using EmployeeRabbitMq.Api.Services;
using EmployeeRabbitMq.Contracts.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var jwtTokenService = new JwtTokenService(builder.Configuration);

builder.Services.AddSingleton(jwtTokenService);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            jwtTokenService.GetValidationParameters();
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<RabbitMqProducer>();
builder.Services.AddSingleton<JwtTokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();