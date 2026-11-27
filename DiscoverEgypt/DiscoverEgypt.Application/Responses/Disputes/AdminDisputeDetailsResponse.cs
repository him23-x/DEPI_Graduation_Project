using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Responses.Disputes
{
    public sealed record AdminDisputeDetailsResponse(
        int DisputeId,
        int BookingId,
        BookingType BookingType,
        string ServiceTitle,
        string TouristName,
        string ProviderName,
        string OpenedByName,
        string Description,
        DisputeStatus DisputeStatus,
        BookingStatus BookingStatus,
        DateTime ScheduledStart,
        DateTime ScheduledEnd,
        decimal ServiceAmount,
        decimal PlatformFeeAmount,
        decimal TotalAmount,
        PaymentStatus PaymentStatus,
        decimal? PayoutAmount,
        PayoutStatus? PayoutStatus,
        DateTime CreatedAt);
}
