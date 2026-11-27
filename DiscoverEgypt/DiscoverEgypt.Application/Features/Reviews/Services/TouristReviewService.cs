using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Reviews;
using DiscoverEgypt.Application.Features.Reviews.Policies;
using DiscoverEgypt.Application.Responses.Reviews;

namespace DiscoverEgypt.Application.Features.Reviews.Services
{
    public sealed class TouristReviewService
    {
        private readonly ITouristReviewRepository _repository;
        private readonly TimeProvider _timeProvider;

        public TouristReviewService(ITouristReviewRepository repository, TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<TouristReviewCreateResponse> CreateAsync(
            string userId,
            int bookingId,
            int rating,
            string? comment,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            if (bookingId <= 0)
                return new TouristReviewCreateResponse(TouristReviewCreateOutcome.BookingUnavailable, bookingId);

            var booking = await _repository.GetOwnedBookingAsync(userId, bookingId, cancellationToken);
            if (booking is null)
                return new TouristReviewCreateResponse(TouristReviewCreateOutcome.BookingUnavailable, bookingId);
            if (booking.BookingStatus != Domain.Enums.Bookings.BookingStatus.Completed)
                return new TouristReviewCreateResponse(TouristReviewCreateOutcome.BookingNotCompleted, bookingId);
            if (!TouristReviewEligibilityPolicy.CanReview(booking.BookingStatus, booking.HasReview))
                return new TouristReviewCreateResponse(TouristReviewCreateOutcome.AlreadyReviewed, bookingId);

            var normalizedComment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
            return await _repository.CreateAsync(
                userId,
                bookingId,
                rating,
                normalizedComment,
                _timeProvider.GetUtcNow().UtcDateTime,
                cancellationToken);
        }
    }
}
