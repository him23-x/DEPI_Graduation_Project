using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Presentation.ViewModels.Dashboard;

public sealed class HostDashboardViewModel
{
    public IReadOnlyList<HostDashboardExperienceStatusViewModel> ExperienceStatuses { get; init; } = [];
    public int PendingRequestCount { get; init; }
    public IReadOnlyList<HostDashboardBookingViewModel> NearestPendingRequests { get; init; } = [];
    public int ConfirmedBookingCount { get; init; }
    public IReadOnlyList<HostDashboardBookingViewModel> UpcomingConfirmedBookings { get; init; } = [];
    public decimal PendingPayoutAmount { get; init; }
    public decimal PaidPayoutAmount { get; init; }
    public decimal CancelledPayoutAmount { get; init; }
    public int UnreadNotificationCount { get; init; }
    public decimal AverageRating { get; init; }
    public int ReviewCount { get; init; }
}
