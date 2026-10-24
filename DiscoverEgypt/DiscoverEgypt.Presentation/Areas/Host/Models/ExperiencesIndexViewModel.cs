using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Presentation.Areas.Host.Models;

public sealed class ExperiencesIndexViewModel
{
    public ExperienceStatus? SelectedStatus { get; init; }
    public IReadOnlyList<HostExperienceListItem> Experiences { get; init; } = [];
    public IReadOnlyList<ExperienceStatus> AvailableStatuses { get; } = Enum.GetValues<ExperienceStatus>();
}
