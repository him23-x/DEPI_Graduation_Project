using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceLookup(int Id, string Name);

public sealed record HostExperienceDraftEditor(
    int ExperienceId,
    ExperienceStatus Status,
    string? RejectionReason,
    CreateHostExperienceDraftRequest Draft);

public sealed record HostExperienceCreateOptions(
    IReadOnlyList<HostExperienceLookup> ExperienceTypes,
    IReadOnlyList<HostExperienceLookup> CulturalAreas,
    IReadOnlyList<HostExperienceLookup> Interests,
    IReadOnlyList<HostExperienceLookup> Languages);
