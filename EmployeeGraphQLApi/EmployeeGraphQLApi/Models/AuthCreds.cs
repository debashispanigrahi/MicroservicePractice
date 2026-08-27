namespace EmployeeGraphQLApi.Models;

public sealed class AuthCreds
{
    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
