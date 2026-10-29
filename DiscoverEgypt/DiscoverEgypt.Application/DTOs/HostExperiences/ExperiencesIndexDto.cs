
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record ExperiencesIndexDto
{
    public ExperienceStatus? SelectedStatus { get; init; }
    public IReadOnlyList<HostExperienceListItemDto> Experiences { get; init; } = [];
    public IReadOnlyList<ExperienceStatus> AvailableStatuses { get; } = Enum.GetValues<ExperienceStatus>();
}

