namespace DiscoverEgypt.Presentation.ViewModels.Shared
{
    public sealed record AccountMenuViewModel(
        string DisplayName,
        string Initial,
        string Role,
        bool HasProfileImage,
        string Placement);
}
