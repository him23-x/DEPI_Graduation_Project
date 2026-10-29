
namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceMediaEditorDto(
    int ExperienceId,
    string Title,
    IReadOnlyList<HostExperienceMediaItemDto> Media);
