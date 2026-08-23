using EmployeeGraphQLApi.Models;
using EmployeeGraphQLApi.Services;

namespace EmployeeGraphQLApi.GraphQL;

public class Query
{
    public async Task<IEnumerable<Employee>> GetEmployees(
        [Service] IEmployeeService employeeService)
    {
        return await employeeService.GetAllAsync();
    }

    public async Task<Employee?> GetEmployee(
        int id,
        [Service] IEmployeeService employeeService)
    {
        return await employeeService.GetByIdAsync(id);
    }
}