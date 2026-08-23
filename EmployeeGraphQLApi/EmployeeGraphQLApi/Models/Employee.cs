namespace EmployeeGraphQLApi.Models;

public class Employee
{
    public int EmployeeId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedAt { get; set; }
}
