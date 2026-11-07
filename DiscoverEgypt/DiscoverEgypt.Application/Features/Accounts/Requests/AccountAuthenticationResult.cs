namespace DiscoverEgypt.Application.Features.Accounts.Requests
{
    public sealed record AccountAuthenticationResult(bool Succeeded, bool IsHost, IReadOnlyList<string> Errors);
}
