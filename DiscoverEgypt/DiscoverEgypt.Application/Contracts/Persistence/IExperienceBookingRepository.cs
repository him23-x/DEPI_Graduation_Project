using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IExperienceBookingRepository
    {
        Task<ExperienceBookingDataDto?> GetPublishedExperienceAsync(int experienceId, CancellationToken cancellationToken = default);
        Task<bool> HasConfirmedOverlapAsync(string providerUserId, DateTime scheduledStartUtc, DateTime scheduledEndUtc, CancellationToken cancellationToken = default);
        Task<int> CreateAsync(Booking booking, CancellationToken cancellationToken = default);
        Task<ExperienceBookingConfirmationResponse?> GetTouristConfirmationAsync(string userId, int bookingId, CancellationToken cancellationToken = default);
    }
}
