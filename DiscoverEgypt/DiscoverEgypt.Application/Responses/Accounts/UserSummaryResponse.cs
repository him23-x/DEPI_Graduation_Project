namespace DiscoverEgypt.Application.Responses.Accounts
{
    public sealed record UserSummaryResponse(
        string Id,
        string UserName,
        string Email,
        string FullName);
}
