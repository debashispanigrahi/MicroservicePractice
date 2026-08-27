using EmployeeGraphQLApi.Models;
using EmployeeGraphQLApi.Services;
using HotChocolate.Authorization;
using Microsoft.Extensions.Options;

namespace EmployeeGraphQLApi.GraphQL.Auth;

[ExtendObjectType("Mutation")]
public class AuthMutations
{
    [AllowAnonymous]
    public AuthToken Login(
        AuthCreds input,
        [Service] IOptions<AuthCreds> configuredCredentials,
        [Service] IJwtTokenService jwtTokenService)
    {
        var credentials = configuredCredentials.Value;

        if (input.UserName != credentials.UserName || input.Password != credentials.Password)
        {
            throw new GraphQLException("Invalid username or password.");
        }

        return jwtTokenService.CreateToken(input.UserName);
    }
}
