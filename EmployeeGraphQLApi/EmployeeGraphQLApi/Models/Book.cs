namespace EmployeeGraphQLApi.Models;

public class Book
{
    public int BookId { get; set; }
    public string BookName { get; set; }
    public string BookDescription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}