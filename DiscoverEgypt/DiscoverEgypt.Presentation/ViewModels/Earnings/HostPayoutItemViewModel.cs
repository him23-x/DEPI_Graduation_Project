using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Presentation.ViewModels.Earnings;

public sealed class HostPayoutItemViewModel
{
    public int BookingId { get; init; }
    public string ExperienceTitle { get; init; } = string.Empty;
    public DateTime ScheduledStart { get; init; }
    public decimal Amount { get; init; }
    public PayoutStatus Status { get; init; }
    public DateTime EligibleAt { get; init; }
    public DateTime? PaidAt { get; init; }
}
