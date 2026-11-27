using DiscoverEgypt.Domain.Enums.Bookings;

namespace DiscoverEgypt.Application.Responses.Reviews
{
    public sealed record TouristReviewBookingResponse(
        int BookingId,
        string BookingTitle,
        BookingStatus BookingStatus,
        bool HasReview);
}
