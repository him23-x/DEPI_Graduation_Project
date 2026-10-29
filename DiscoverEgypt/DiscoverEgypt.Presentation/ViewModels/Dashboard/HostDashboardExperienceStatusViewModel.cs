using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Presentation.ViewModels.Dashboard;

public sealed class HostDashboardExperienceStatusViewModel
{
    public ExperienceStatus Status { get; init; }
    public int Count { get; init; }
}
