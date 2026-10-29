using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.DTOs.Disputes;

public sealed record HostDisputeListItemDto(
    int DisputeId,
    int BookingId,
    string ExperienceTitle,
    string GuestName,
    DateTime ScheduledStart,
    DateTime CreatedAt,
    DisputeStatus Status,
    DisputeOutcome? Outcome,
    PayoutStatus? PayoutStatus,
    decimal? PayoutAmount,
    DateTime? EligibleAt,
    bool IsPayoutHeld);
