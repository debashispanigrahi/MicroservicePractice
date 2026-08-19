using Dapper;
using Microsoft.Data.SqlClient;
using WebSocketEmployeeApi.Models;

namespace WebSocketEmployeeApi.Services;

public interface IEmployeeService
{
    Task<int> AddEmployeeAsync(CreateEmployeeRequest employee);
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<int> AddEmployeesAsync(IEnumerable<CreateEmployeeRequest> employees);
}

public class EmployeeService(IConfiguration configuration) : IEmployeeService
{
    private SqlConnection CreateConnection()
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        return new SqlConnection(connectionString);
    }

    public async Task<int> AddEmployeeAsync(
        CreateEmployeeRequest employee)
    {
        const string sql = """
            INSERT INTO Employees
            (
                Name,
                Email,
                CreatedAt
            )
            OUTPUT INSERTED.EmployeeId
            VALUES
            (
                @Name,
                @Email,
                @CreatedAt
            );
            """;

        var parameters = new
        {
            employee.Name,
            employee.Email,
            CreatedAt = DateTime.Now
        };

        await using var connection = CreateConnection();

        return await connection.ExecuteScalarAsync<int>(
            sql,
            parameters);
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        const string sql = """
            SELECT
                EmployeeId,
                Name,
                Email,
                CreatedAt
            FROM Employees
            ORDER BY EmployeeId;
            """;

        await using var connection = CreateConnection();

        return await connection.QueryAsync<Employee>(sql);
    }

    public async Task<int> AddEmployeesAsync(
        IEnumerable<CreateEmployeeRequest> employees)
    {
        const string sql = """
            INSERT INTO Employees
            (
                Name,
                Email,
                CreatedAt
            )
            VALUES
            (
                @Name,
                @Email,
                @CreatedAt
            );
            """;

        var parameters = employees.Select(employee => new
        {
            employee.Name,
            employee.Email,
            CreatedAt = DateTime.Now
        }).ToList();

        await using var connection = CreateConnection();

        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
            var count = await connection.ExecuteAsync(
                sql,
                parameters,
                transaction);

            transaction.Commit();

            return count;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}