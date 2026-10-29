
namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceCreateOptionsDto(
    IReadOnlyList<HostExperienceLookupDto> ExperienceTypes,
    IReadOnlyList<HostExperienceLookupDto> CulturalAreas,
    IReadOnlyList<HostExperienceLookupDto> Interests,
    IReadOnlyList<HostExperienceLookupDto> Languages);
