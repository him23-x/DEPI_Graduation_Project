using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;

namespace DiscoverEgypt.Application.Responses.Disputes
{
    public sealed record TouristDisputeDetailsResponse(
        int DisputeId,
        int BookingId,
        BookingType BookingType,
        string BookingTitle,
        DateTime ScheduledStartUtc,
        DateTime ScheduledEndUtc,
        BookingStatus BookingStatus,
        string Description,
        DisputeStatus Status,
        DisputeOutcome? Outcome,
        string? ResolutionNote,
        DateTime CreatedAtUtc,
        DateTime? ResolvedAtUtc);
}
