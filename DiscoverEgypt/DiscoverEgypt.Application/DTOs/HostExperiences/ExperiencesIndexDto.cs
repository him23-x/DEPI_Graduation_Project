using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed class ExperiencesIndexDto
{
    public ExperienceStatus? SelectedStatus { get; init; }
    public IReadOnlyList<HostExperienceListItem> Experiences { get; init; } = [];
    public IReadOnlyList<ExperienceStatus> AvailableStatuses { get; } = Enum.GetValues<ExperienceStatus>();
}

