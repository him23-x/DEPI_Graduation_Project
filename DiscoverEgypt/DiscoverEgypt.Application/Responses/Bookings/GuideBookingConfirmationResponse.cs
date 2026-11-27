using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record GuideBookingConfirmationResponse(
        int BookingId,
        string GuideName,
        DateTime ScheduledStartUtc,
        DateTime ScheduledEndUtc,
        int GuideHours,
        string Destination,
        decimal HourlyRate,
        decimal ServiceAmount,
        decimal PlatformFeeRate,
        decimal PlatformFeeAmount,
        decimal TotalAmount,
        BookingStatus BookingStatus,
        PaymentStatus PaymentStatus);
}
