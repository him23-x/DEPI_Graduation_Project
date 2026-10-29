namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed record ExperienceMediaViewModel(
    int ExperienceId,
    string Title,
    IReadOnlyList<ExperienceMediaItemViewModel> Media);
