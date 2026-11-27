using DiscoverEgypt.Application.DTOs.Reviews;
using DiscoverEgypt.Application.Responses.Bookings;

namespace DiscoverEgypt.Application.Responses.Reviews
{
    public sealed record TouristReviewFormResponse(
        TouristBookingDetailsResponse Booking,
        CreateTouristReviewDto Form);
}
