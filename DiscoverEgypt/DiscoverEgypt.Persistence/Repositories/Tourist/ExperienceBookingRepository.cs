using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Experiences;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Tourist
{
    public sealed class ExperienceBookingRepository : IExperienceBookingRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public ExperienceBookingRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public Task<ExperienceBookingDataDto?> GetPublishedExperienceAsync(int experienceId, CancellationToken cancellationToken = default) =>
            _dbContext.Experiences.AsNoTracking()
                .Where(experience => experience.Id == experienceId && experience.Status == ExperienceStatus.Published)
                .Select(experience => new ExperienceBookingDataDto(
                    experience.Id,
                    experience.Title,
                    experience.ExperienceType.Name,
                    experience.CulturalArea.Name,
                    experience.MeetingPointArea,
                    _dbContext.ApplicationUsers.Where(user => user.Id == experience.HostProfile.UserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? string.Empty,
                    experience.HostProfile.UserId,
                    experience.PricePerPerson,
                    experience.DurationMinutes,
                    experience.MaxGroupSize))
                .SingleOrDefaultAsync(cancellationToken);

        public Task<bool> HasConfirmedOverlapAsync(
            string providerUserId,
            DateTime scheduledStartUtc,
            DateTime scheduledEndUtc,
            CancellationToken cancellationToken = default) =>
            _dbContext.Bookings.AsNoTracking().AnyAsync(booking =>
                booking.ProviderUserId == providerUserId
                && booking.Status == BookingStatus.Confirmed
                && booking.ScheduledStart < scheduledEndUtc
                && booking.ScheduledEnd > scheduledStartUtc,
                cancellationToken);

        public async Task<int> CreateAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return booking.Id;
        }

        public Task<ExperienceBookingConfirmationResponse?> GetTouristConfirmationAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken = default) =>
            _dbContext.Bookings.AsNoTracking()
                .Where(booking => booking.Id == bookingId
                    && booking.TouristUserId == userId
                    && booking.BookingType == BookingType.Experience
                    && booking.Experience != null)
                .Select(booking => new ExperienceBookingConfirmationResponse(
                    booking.Id,
                    booking.Experience!.Title,
                    booking.Experience.ExperienceType.Name,
                    booking.Experience.CulturalArea.Name,
                    booking.Experience.MeetingPointArea,
                    booking.ScheduledStart,
                    booking.ScheduledEnd,
                    booking.NumberOfGuests ?? 0,
                    booking.PricePerPersonSnapshot ?? 0m,
                    booking.ServiceAmount,
                    booking.PlatformFeeRate,
                    booking.PlatformFeeAmount,
                    booking.TotalAmount,
                    booking.Status,
                    booking.Payment.Status))
                .SingleOrDefaultAsync(cancellationToken);
    }
}
