using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;

namespace DiscoverEgypt.Application.Responses.Disputes
{
    public sealed record AdminDisputeQueueItemResponse(
        int DisputeId,
        int BookingId,
        string ServiceTitle,
        BookingType BookingType,
        string TouristName,
        string ProviderName,
        BookingStatus BookingStatus,
        DisputeStatus DisputeStatus,
        DateTime CreatedAt);
}
