
namespace DiscoverEgypt.Application.ViewModels.HostExperiences
{
    public sealed record ExperienceCreateOptionsViewModel(
        IReadOnlyList<ExperienceLookupViewModel> ExperienceTypes,
        IReadOnlyList<ExperienceLookupViewModel> CulturalAreas,
        IReadOnlyList<ExperienceLookupViewModel> Interests,
        IReadOnlyList<ExperienceLookupViewModel> Languages)
    {
        public bool HasRequiredLookups => ExperienceTypes.Count > 0
            && CulturalAreas.Count > 0
            && Interests.Count > 0
            && Languages.Count > 0;
    }
}

