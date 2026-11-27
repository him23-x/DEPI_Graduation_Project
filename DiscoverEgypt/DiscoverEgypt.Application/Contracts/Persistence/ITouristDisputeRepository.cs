using DiscoverEgypt.Application.Enums.Disputes;
using DiscoverEgypt.Application.Responses.Disputes;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface ITouristDisputeRepository
    {
        Task<TouristDisputeBookingResponse?> GetOwnedBookingAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken = default);

        Task<TouristDisputeOpenResponse> OpenAsync(
            string userId,
            int bookingId,
            string description,
            DateTime nowUtc,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TouristDisputeListItemResponse>> GetForTouristAsync(
            string userId,
            CancellationToken cancellationToken = default);

        Task<TouristDisputeDetailsResponse?> GetDetailsForTouristAsync(
            string userId,
            int disputeId,
            CancellationToken cancellationToken = default);
    }
}
