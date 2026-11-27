using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Verification;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Tourist
{
    public sealed class GuideBookingRepository : IGuideBookingRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public GuideBookingRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public Task<GuideBookingDataDto?> GetApprovedGuideAsync(
            int guideProfileId,
            DateOnly todayUtc,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.GuideProfiles.AsNoTracking()
                .Where(guide => guide.Id == guideProfileId)
                .Where(guide => guide.VerificationRequests
                    .OrderByDescending(request => request.SubmittedAt)
                    .ThenByDescending(request => request.Id)
                    .Select(request => request.Status)
                    .FirstOrDefault() == VerificationStatus.Approved)
                .Where(guide => guide.VerificationRequests
                    .OrderByDescending(request => request.SubmittedAt)
                    .ThenByDescending(request => request.Id)
                    .Select(request => (DateOnly?)request.LicenseExpiryDate)
                    .FirstOrDefault() >= todayUtc)
                .Select(guide => new GuideBookingDataDto(
                    guide.Id,
                    _dbContext.ApplicationUsers.Where(user => user.Id == guide.UserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? string.Empty,
                    guide.Bio,
                    guide.HourlyRate,
                    guide.ProfileImageKey,
                    guide.GuideLanguages.Select(link => link.Language.Name).OrderBy(name => name).ToList(),
                    guide.GuideCulturalAreas.Select(link => link.CulturalArea.Name).OrderBy(name => name).ToList(),
                    guide.Bookings.Where(booking => booking.Review != null)
                        .Select(booking => (decimal?)booking.Review!.Rating).Average() ?? 0m,
                    guide.Bookings.Count(booking => booking.Review != null),
                    guide.UserId))
                .SingleOrDefaultAsync(cancellationToken);
        }

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

        public Task<GuideBookingConfirmationResponse?> GetTouristConfirmationAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken = default) =>
            _dbContext.Bookings.AsNoTracking()
                .Where(booking => booking.Id == bookingId
                    && booking.TouristUserId == userId
                    && booking.BookingType == BookingType.Guide
                    && booking.GuideProfile != null)
                .Select(booking => new GuideBookingConfirmationResponse(
                    booking.Id,
                    _dbContext.ApplicationUsers.Where(user => user.Id == booking.GuideProfile!.UserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? string.Empty,
                    booking.ScheduledStart,
                    booking.ScheduledEnd,
                    booking.GuideHours ?? 0,
                    booking.Destination ?? string.Empty,
                    booking.HourlyRateSnapshot ?? 0m,
                    booking.ServiceAmount,
                    booking.PlatformFeeRate,
                    booking.PlatformFeeAmount,
                    booking.TotalAmount,
                    booking.Status,
                    booking.Payment.Status))
                .SingleOrDefaultAsync(cancellationToken);
    }
}
