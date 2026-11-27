using DiscoverEgypt.Application.Enums.AdminUsers;

namespace DiscoverEgypt.Application.Responses.AdminUsers
{
    public sealed record AdminUserStateChangeResponse(AdminUserStateChangeOutcome Outcome, string UserId, bool IsActive);
}
