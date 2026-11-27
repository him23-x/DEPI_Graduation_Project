using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IGuideBookingRepository
    {
        Task<GuideBookingDataDto?> GetApprovedGuideAsync(int guideProfileId, DateOnly todayUtc, CancellationToken cancellationToken = default);
        Task<bool> HasConfirmedOverlapAsync(string providerUserId, DateTime scheduledStartUtc, DateTime scheduledEndUtc, CancellationToken cancellationToken = default);
        Task<int> CreateAsync(Booking booking, CancellationToken cancellationToken = default);
        Task<GuideBookingConfirmationResponse?> GetTouristConfirmationAsync(string userId, int bookingId, CancellationToken cancellationToken = default);
    }
}
