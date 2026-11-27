
namespace DiscoverEgypt.Application.DTOs.HostExperiences
{
    public sealed record ExperienceCreateOptionsDto(
        IReadOnlyList<ExperienceLookupDto> ExperienceTypes,
        IReadOnlyList<ExperienceLookupDto> CulturalAreas,
        IReadOnlyList<ExperienceLookupDto> Interests,
        IReadOnlyList<ExperienceLookupDto> Languages)
    {
        public bool HasRequiredLookups => ExperienceTypes.Count > 0
            && CulturalAreas.Count > 0
            && Interests.Count > 0
            && Languages.Count > 0;
    }
}

