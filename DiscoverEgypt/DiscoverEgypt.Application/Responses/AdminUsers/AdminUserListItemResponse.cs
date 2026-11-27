namespace DiscoverEgypt.Application.Responses.AdminUsers
{
    public sealed record AdminUserListItemResponse(
        string UserId,
        string FullName,
        string Email,
        IReadOnlyList<string> Roles,
        bool IsActive,
        DateTime CreatedAt);
}
