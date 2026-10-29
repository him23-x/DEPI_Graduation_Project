using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Presentation.ViewModels.Disputes;

public sealed class HostDisputeListItemViewModel
{
    public int DisputeId { get; init; }
    public int BookingId { get; init; }
    public string ExperienceTitle { get; init; } = string.Empty;
    public string GuestName { get; init; } = string.Empty;
    public DateTime ScheduledStart { get; init; }
    public DateTime CreatedAt { get; init; }
    public DisputeStatus Status { get; init; }
    public DisputeOutcome? Outcome { get; init; }
    public PayoutStatus? PayoutStatus { get; init; }
    public decimal? PayoutAmount { get; init; }
    public DateTime? EligibleAt { get; init; }
    public bool IsPayoutHeld { get; init; }
}
