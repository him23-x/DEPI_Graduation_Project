using DiscoverEgypt.Application.Enums.Reviews;
using DiscoverEgypt.Application.Responses.Reviews;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface ITouristReviewRepository
    {
        Task<TouristReviewBookingResponse?> GetOwnedBookingAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken = default);

        Task<TouristReviewCreateResponse> CreateAsync(
            string userId,
            int bookingId,
            int rating,
            string? comment,
            DateTime nowUtc,
            CancellationToken cancellationToken = default);
    }
}
