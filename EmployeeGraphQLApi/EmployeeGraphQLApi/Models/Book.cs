namespace EmployeeGraphQLApi.Models;

public class Book
{
    public int? BookId { get; set; }
    public required string BookName { get; set; }
    public required string BookDescription { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
}