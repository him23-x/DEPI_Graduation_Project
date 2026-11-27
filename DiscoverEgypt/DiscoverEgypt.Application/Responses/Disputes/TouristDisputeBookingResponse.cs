using DiscoverEgypt.Domain.Enums.Bookings;

namespace DiscoverEgypt.Application.Responses.Disputes
{
    public sealed record TouristDisputeBookingResponse(
        int BookingId,
        BookingType BookingType,
        string BookingTitle,
        DateTime ScheduledStartUtc,
        DateTime ScheduledEndUtc,
        BookingStatus BookingStatus,
        decimal? PayoutAmount);
}
