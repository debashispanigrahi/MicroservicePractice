namespace EmployeeRabbitMq.Contracts.Models;

public class TokenRequest
{
    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = "EmployeeManager";
}