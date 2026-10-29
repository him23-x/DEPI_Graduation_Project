namespace DiscoverEgypt.Application.DTOs.Dashboard;

public sealed record HostDashboardBookingDto(
    int BookingId,
    string ExperienceTitle,
    DateTime ScheduledStart);
