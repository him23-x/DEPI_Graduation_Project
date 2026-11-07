
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.ViewModels.HostExperiences
{
    public sealed record ExperiencesIndexViewModel
    {
        public ExperienceStatus? SelectedStatus { get; init; }
        public IReadOnlyList<ExperienceListItemViewModel> Experiences { get; init; } = [];
        public IReadOnlyList<ExperienceStatus> AvailableStatuses { get; } = Enum.GetValues<ExperienceStatus>();
    }
}

