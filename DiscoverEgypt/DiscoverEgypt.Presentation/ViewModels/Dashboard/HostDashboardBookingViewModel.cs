namespace DiscoverEgypt.Presentation.ViewModels.Dashboard;

public sealed class HostDashboardBookingViewModel
{
    public int BookingId { get; init; }
    public string ExperienceTitle { get; init; } = string.Empty;
    public DateTime ScheduledStart { get; init; }
}
