using EmployeeGraphQLApi.Services;
using HotChocolate.Authorization;

namespace EmployeeGraphQLApi.GraphQL.Employee;

[Authorize]
[ExtendObjectType("Query")]
public class EmployeeQueries
{
    public async Task<IEnumerable<Models.Employee>> GetEmployees(
        [Service] IEmployeeService employeeService)
    {
        return await employeeService.GetAllAsync();
    }

    public async Task<Models.Employee?> GetEmployee(
        int id,
        [Service] IEmployeeService employeeService)
    {
        return await employeeService.GetByIdAsync(id);
    }
}