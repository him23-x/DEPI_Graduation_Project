using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceDraftEditor(
    int ExperienceId,
    ExperienceStatus Status,
    string? RejectionReason,
    CreateHostExperienceDraftRequest Draft);

