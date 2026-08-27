using EmployeeGraphQLApi.GraphQL.Auth;
using EmployeeGraphQLApi.GraphQL.Employee;
using HotChocolate.Execution.Configuration;

namespace EmployeeGraphQLApi.GraphQL;

public static class GraphQLExtensions
{
public static IRequestExecutorBuilder AddAllGraphQL(
        this IRequestExecutorBuilder builder)
    {
        return builder
            .AddTypeExtension<EmployeeQueries>()
            .AddTypeExtension<EmployeeMutations>()
            .AddTypeExtension<AuthMutations>();
    }
}
