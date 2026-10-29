using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.DTOs.Disputes;

public sealed record HostDisputeDetailsDto(
    int DisputeId,
    int BookingId,
    string ExperienceTitle,
    string GuestName,
    DateTime ScheduledStart,
    DateTime ScheduledEnd,
    int NumberOfGuests,
    decimal ServiceAmount,
    BookingStatus BookingStatus,
    DisputeStatus Status,
    DisputeOutcome? Outcome,
    string Description,
    string? ResolutionNote,
    DateTime CreatedAt,
    DateTime? ResolvedAt,
    decimal? PayoutAmount,
    PayoutStatus? PayoutStatus,
    DateTime? EligibleAt,
    DateTime? PaidAt,
    bool IsPayoutHeld);
