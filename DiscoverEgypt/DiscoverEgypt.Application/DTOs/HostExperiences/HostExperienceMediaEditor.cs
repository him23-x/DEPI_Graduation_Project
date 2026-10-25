using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceMediaEditor(
    int ExperienceId,
    string Title,
    IReadOnlyList<HostExperienceMediaItem> Media);

