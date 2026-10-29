
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed class ExperiencesIndexViewModel
{
    public ExperienceStatus? SelectedStatus { get; init; }
    public IReadOnlyList<ExperienceListItemViewModel> Experiences { get; init; } = [];
    public IReadOnlyList<ExperienceStatus> AvailableStatuses { get; } = Enum.GetValues<ExperienceStatus>();
}
