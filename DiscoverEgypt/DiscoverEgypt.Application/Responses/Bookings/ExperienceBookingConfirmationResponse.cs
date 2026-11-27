using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record ExperienceBookingConfirmationResponse(
        int BookingId,
        string Title,
        string ExperienceType,
        string CulturalArea,
        string MeetingPointArea,
        DateTime ScheduledStartUtc,
        DateTime ScheduledEndUtc,
        int NumberOfGuests,
        decimal PricePerPerson,
        decimal ServiceAmount,
        decimal PlatformFeeRate,
        decimal PlatformFeeAmount,
        decimal TotalAmount,
        BookingStatus BookingStatus,
        PaymentStatus PaymentStatus);
}
