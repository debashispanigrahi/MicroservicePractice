namespace EmployeeRabbitMq.Contracts.Models;

public class EmployeeCreatedMessage
{
    public int EmployeeId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedAt { get; set; }
}