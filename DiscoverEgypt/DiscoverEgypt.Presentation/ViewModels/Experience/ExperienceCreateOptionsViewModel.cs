namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed class ExperienceCreateOptionsViewModel
{
    public IReadOnlyList<ExperienceLookupViewModel> ExperienceTypes { get; init; } = [];
    public IReadOnlyList<ExperienceLookupViewModel> CulturalAreas { get; init; } = [];
    public IReadOnlyList<ExperienceLookupViewModel> Interests { get; init; } = [];
    public IReadOnlyList<ExperienceLookupViewModel> Languages { get; init; } = [];

    public bool HasRequiredLookups => ExperienceTypes.Count > 0
        && CulturalAreas.Count > 0
        && Interests.Count > 0
        && Languages.Count > 0;
}
