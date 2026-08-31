using EmployeeGraphQLApi.Services;
using System.Text.Json;
using HotChocolate.Authorization;

namespace EmployeeGraphQLApi.GraphQL.Employee;

[Authorize]
[ExtendObjectType("Mutation")]
public class EmployeeMutations
{
    public async Task<Models.Employee?> AddEmployee(
        Models.AddEmployee input,
        [Service] IEmployeeService employeeService)
    {
        var employee = new Models.Employee
        {
            Name = input.name,
            Email = input.email
        };
        var employeeId = await employeeService.AddAsync(employee);

        return await employeeService.GetByIdAsync(employeeId);
    }

    public async Task<int> UploadEmployees(IFile file, [Service] IEmployeeService employeeService)
    {
        if (file == null)
        {
            throw new GraphQLException(
                "Employee file is required.");
        }

        if (!file.Name.EndsWith(
                ".txt",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new GraphQLException(
                "Only .txt files are supported.");
        }

        await using var stream = file.OpenReadStream();

        using var reader = new StreamReader(stream);

        var content = await reader.ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new GraphQLException(
                "The uploaded file is empty.");
        }

        List<Models.Employee>? employees;

        try
        {
            employees =
                JsonSerializer.Deserialize<List<Models.Employee>>(
                    content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
        catch (JsonException x)
        {
            throw new GraphQLException(x.Message);
        }

        if (employees == null || employees.Count == 0)
        {
            throw new GraphQLException(
                "No employees were found in the file.");
        }

        return await employeeService.AddEmployeesAsync(
            employees);
    }
}