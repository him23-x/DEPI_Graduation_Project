namespace DiscoverEgypt.Persistence.Identity;

public static class IdentityRoles
{
    public const string Tourist = "Tourist";
    public const string Host = "Host";
    public const string Guide = "Guide";
    public const string Admin = "Admin";

    public static IReadOnlyList<string> All { get; } = [Tourist, Host, Guide, Admin];
}
