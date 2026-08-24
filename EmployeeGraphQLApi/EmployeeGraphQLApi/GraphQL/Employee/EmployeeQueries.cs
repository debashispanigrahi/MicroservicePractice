using EmployeeGraphQLApi.Services;

namespace EmployeeGraphQLApi.GraphQL.Employee;

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