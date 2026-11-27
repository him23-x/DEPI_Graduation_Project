using DiscoverEgypt.Application.Enums.Reviews;

namespace DiscoverEgypt.Application.Responses.Reviews
{
    public sealed record TouristReviewCreateResponse(
        TouristReviewCreateOutcome Outcome,
        int BookingId);
}
