namespace DiscoverEgypt.Application.ViewModels.Dashboard
{
    public sealed record HostDashboardBookingViewModel(
        int BookingId,
        string ExperienceTitle,
        DateTime ScheduledStart);
}

