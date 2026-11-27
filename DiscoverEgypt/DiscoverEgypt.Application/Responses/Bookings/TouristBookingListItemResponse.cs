using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record TouristBookingListItemResponse(
        int BookingId,
        BookingType BookingType,
        string Title,
        DateTime ScheduledStartUtc,
        DateTime ScheduledEndUtc,
        int? NumberOfGuests,
        int? GuideHours,
        BookingStatus BookingStatus,
        PaymentStatus PaymentStatus,
        decimal TotalAmount,
        decimal RefundedAmount,
        PayoutStatus? PayoutStatus);
}
