using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed class ExperiencesIndexViewModel
{
    public ExperienceStatus? SelectedStatus { get; init; }
    public IReadOnlyList<HostExperienceListItem> Experiences { get; init; } = [];
    public IReadOnlyList<ExperienceStatus> AvailableStatuses { get; } = Enum.GetValues<ExperienceStatus>();
}
