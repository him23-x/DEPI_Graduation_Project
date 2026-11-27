namespace DiscoverEgypt.Application.Responses.Dashboard
{
    public sealed record HostDashboardResponse(
        IReadOnlyList<HostDashboardExperienceStatusResponse> ExperienceStatuses,
        int PendingRequestCount,
        IReadOnlyList<HostDashboardBookingResponse> NearestPendingRequests,
        int ConfirmedBookingCount,
        IReadOnlyList<HostDashboardBookingResponse> UpcomingConfirmedBookings,
        decimal PendingPayoutAmount,
        decimal PaidPayoutAmount,
        decimal CancelledPayoutAmount,
        int UnreadNotificationCount,
        decimal AverageRating,
        int ReviewCount);
}
