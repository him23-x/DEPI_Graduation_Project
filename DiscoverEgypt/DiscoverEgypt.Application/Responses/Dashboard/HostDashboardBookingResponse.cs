namespace DiscoverEgypt.Application.Responses.Dashboard
{
    public sealed record HostDashboardBookingResponse(
        int BookingId,
        string ExperienceTitle,
        DateTime ScheduledStart);
}
