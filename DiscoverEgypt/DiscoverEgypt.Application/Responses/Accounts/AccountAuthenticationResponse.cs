namespace DiscoverEgypt.Application.Responses.Accounts
{
    public sealed record AccountAuthenticationResponse(
        bool Succeeded,
        bool IsHost,
        IReadOnlyList<string> Errors,
        bool IsTourist = false);
}
