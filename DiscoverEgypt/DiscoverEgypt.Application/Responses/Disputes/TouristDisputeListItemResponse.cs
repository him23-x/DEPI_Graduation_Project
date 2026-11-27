using DiscoverEgypt.Domain.Enums.Disputes;

namespace DiscoverEgypt.Application.Responses.Disputes
{
    public sealed record TouristDisputeListItemResponse(
        int DisputeId,
        int BookingId,
        string BookingTitle,
        DateTime ScheduledStartUtc,
        DateTime CreatedAtUtc,
        DisputeStatus Status);
}
