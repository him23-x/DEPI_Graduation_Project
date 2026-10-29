
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceDraftEditorDto(
    int ExperienceId,
    ExperienceStatus Status,
    string? RejectionReason,
    CreateHostExperienceDraftDto Draft);

