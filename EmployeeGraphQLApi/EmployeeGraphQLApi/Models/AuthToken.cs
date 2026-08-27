namespace EmployeeGraphQLApi.Models;

public sealed class AuthToken
{
    public required string AccessToken { get; init; }

    public required DateTime ExpiresAtUtc { get; init; }
}
