using DiscoverEgypt.Application.Enums.Bookings;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface ITouristBookingRepository
    {
        Task<IReadOnlyList<TouristBookingListItemResponse>> GetForTouristAsync(string userId, CancellationToken cancellationToken = default);
        Task<TouristBookingDetailsResponse?> GetDetailsForTouristAsync(string userId, int bookingId, CancellationToken cancellationToken = default);
        Task<BookingCancellationOutcome> CancelAsync(
            string userId,
            int bookingId,
            BookingStatus expectedStatus,
            decimal refundAmount,
            decimal providerAmount,
            PaymentStatus? updatedPaymentStatus,
            DateTime nowUtc,
            CancellationToken cancellationToken = default);
    }
}
