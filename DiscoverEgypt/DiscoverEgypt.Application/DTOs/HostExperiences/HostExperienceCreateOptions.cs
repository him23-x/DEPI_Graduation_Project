using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceCreateOptions(
    IReadOnlyList<HostExperienceLookup> ExperienceTypes,
    IReadOnlyList<HostExperienceLookup> CulturalAreas,
    IReadOnlyList<HostExperienceLookup> Interests,
    IReadOnlyList<HostExperienceLookup> Languages);
