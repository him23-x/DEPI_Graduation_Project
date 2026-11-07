namespace DiscoverEgypt.Application.ViewModels.Dashboard
{
    public sealed record HostDashboardViewModel(
        IReadOnlyList<HostDashboardExperienceStatusViewModel> ExperienceStatuses,
        int PendingRequestCount,
        IReadOnlyList<HostDashboardBookingViewModel> NearestPendingRequests,
        int ConfirmedBookingCount,
        IReadOnlyList<HostDashboardBookingViewModel> UpcomingConfirmedBookings,
        decimal PendingPayoutAmount,
        decimal PaidPayoutAmount,
        decimal CancelledPayoutAmount,
        int UnreadNotificationCount,
        decimal AverageRating,
        int ReviewCount);
}

