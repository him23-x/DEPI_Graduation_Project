namespace DiscoverEgypt.Application.DTOs.Dashboard;

public sealed record HostDashboardDto(
    IReadOnlyList<HostDashboardExperienceStatusDto> ExperienceStatuses,
    int PendingRequestCount,
    IReadOnlyList<HostDashboardBookingDto> NearestPendingRequests,
    int ConfirmedBookingCount,
    IReadOnlyList<HostDashboardBookingDto> UpcomingConfirmedBookings,
    decimal PendingPayoutAmount,
    decimal PaidPayoutAmount,
    decimal CancelledPayoutAmount,
    int UnreadNotificationCount,
    decimal AverageRating,
    int ReviewCount);
