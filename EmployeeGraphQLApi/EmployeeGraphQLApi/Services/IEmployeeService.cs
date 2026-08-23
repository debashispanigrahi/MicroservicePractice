using Dapper;
using EmployeeGraphQLApi.Models;
using Microsoft.Data.SqlClient;

namespace EmployeeGraphQLApi.Services;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllAsync();

    Task<Employee?> GetByIdAsync(int employeeId);

    Task<int> AddAsync(AddEmployeeInput input);
    Task<int> AddEmployeesAsync(IEnumerable<AddEmployeeInput> employees);
}

public class EmployeeService : IEmployeeService
{
    private readonly IConfiguration _configuration;

    public EmployeeService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection CreateConnection()
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        return new SqlConnection(connectionString);
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
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

    public async Task<Employee?> GetByIdAsync(
        int employeeId)
    {
        const string sql = """
            SELECT
                EmployeeId,
                Name,
                Email,
                CreatedAt
            FROM Employees
            WHERE EmployeeId = @EmployeeId;
            """;

        await using var connection = CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Employee>(
            sql,
            new { EmployeeId = employeeId });
    }

    public async Task<int> AddAsync(
        AddEmployeeInput input)
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
            input.Name,
            input.Email,
            CreatedAt = DateTime.UtcNow
        };

        await using var connection = CreateConnection();

        return await connection.ExecuteScalarAsync<int>(
            sql,
            parameters);
    }

    public async Task<int> AddEmployeesAsync(
    IEnumerable<AddEmployeeInput> employees)
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

        var createdAt = DateTime.Now;

        var parameters = employees.Select(employee => new
        {
            employee.Name,
            employee.Email,
            CreatedAt = createdAt
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
