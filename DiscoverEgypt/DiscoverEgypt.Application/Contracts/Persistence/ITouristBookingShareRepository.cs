using DiscoverEgypt.Application.Enums.BookingShares;
using DiscoverEgypt.Application.Responses.BookingShares;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface ITouristBookingShareRepository
    {
        Task<BookingShareCreateResponse> CreateAsync(
            string userId,
            int bookingId,
            string token,
            DateTime nowUtc,
            CancellationToken cancellationToken = default);

        Task<SharedBookingResponse?> GetActiveAsync(
            string token,
            DateTime nowUtc,
            CancellationToken cancellationToken = default);
    }
}
