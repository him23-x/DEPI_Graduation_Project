
namespace DiscoverEgypt.Application.ViewModels.HostExperiences
{
    public sealed record ExperienceMediaViewModel(
        int ExperienceId,
        string Title,
        IReadOnlyList<ExperienceMediaItemViewModel> Media);
}

