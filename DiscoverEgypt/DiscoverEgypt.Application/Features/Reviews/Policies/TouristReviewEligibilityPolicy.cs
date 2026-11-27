using DiscoverEgypt.Domain.Enums.Bookings;

namespace DiscoverEgypt.Application.Features.Reviews.Policies
{
    public static class TouristReviewEligibilityPolicy
    {
        public static bool CanReview(BookingStatus bookingStatus, bool hasReview) =>
            bookingStatus == BookingStatus.Completed && !hasReview;
    }
}
