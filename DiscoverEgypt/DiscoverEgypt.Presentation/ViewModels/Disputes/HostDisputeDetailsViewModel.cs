using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Presentation.ViewModels.Disputes;

public sealed class HostDisputeDetailsViewModel
{
    public int DisputeId { get; init; }
    public int BookingId { get; init; }
    public string ExperienceTitle { get; init; } = string.Empty;
    public string GuestName { get; init; } = string.Empty;
    public DateTime ScheduledStart { get; init; }
    public DateTime ScheduledEnd { get; init; }
    public int NumberOfGuests { get; init; }
    public decimal ServiceAmount { get; init; }
    public BookingStatus BookingStatus { get; init; }
    public DisputeStatus Status { get; init; }
    public DisputeOutcome? Outcome { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? ResolutionNote { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ResolvedAt { get; init; }
    public decimal? PayoutAmount { get; init; }
    public PayoutStatus? PayoutStatus { get; init; }
    public DateTime? EligibleAt { get; init; }
    public DateTime? PaidAt { get; init; }
    public bool IsPayoutHeld { get; init; }
}
