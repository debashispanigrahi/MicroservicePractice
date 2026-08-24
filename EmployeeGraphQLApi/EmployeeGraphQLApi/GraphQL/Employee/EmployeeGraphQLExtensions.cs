using HotChocolate.Execution.Configuration;

namespace EmployeeGraphQLApi.GraphQL.Employee;

public static class EmployeeGraphQLExtensions
{
public static IRequestExecutorBuilder AddEmployeeGraphQL(
        this IRequestExecutorBuilder builder)
    {
        return builder
            .AddTypeExtension<EmployeeQueries>()
            .AddTypeExtension<EmployeeMutations>();
    }
}
