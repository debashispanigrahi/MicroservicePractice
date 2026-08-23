using EmployeeGraphQLApi.Models;
using EmployeeGraphQLApi.Services;
using System.Text.Json;

namespace EmployeeGraphQLApi.GraphQL;

public class Mutation
{
    public async Task<Employee?> AddEmployee(
        AddEmployeeInput input,
        [Service] IEmployeeService employeeService)
    {
        var employeeId = await employeeService.AddAsync(input);

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

        List<AddEmployeeInput>? employees;

        try
        {
            employees =
                JsonSerializer.Deserialize<List<AddEmployeeInput>>(
                    content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
        catch (JsonException)
        {
            throw new GraphQLException(
                "The file does not contain valid employee JSON.");
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