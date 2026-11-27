using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.BookingShares;
using DiscoverEgypt.Application.Responses.BookingShares;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Tourist
{
    public sealed class TouristBookingShareRepository : ITouristBookingShareRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public TouristBookingShareRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public async Task<BookingShareCreateResponse> CreateAsync(
            string userId,
            int bookingId,
            string token,
            DateTime nowUtc,
            CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

            var booking = await _dbContext.Bookings
                .Include(item => item.Payment)
                .Include(item => item.Experience)
                .SingleOrDefaultAsync(item => item.Id == bookingId && item.TouristUserId == userId, cancellationToken);
            if (booking is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return new BookingShareCreateResponse(BookingShareCreateOutcome.BookingUnavailable, null, null);
            }

            var scheduledEndUtc = DateTime.SpecifyKind(booking.ScheduledEnd, DateTimeKind.Utc);
            var location = booking.BookingType == BookingType.Experience
                ? booking.Experience?.ExactMeetingPoint
                : booking.Destination;
            if (booking.Status != BookingStatus.Confirmed
                || booking.Payment.Status != PaymentStatus.Paid
                || scheduledEndUtc <= nowUtc
                || string.IsNullOrWhiteSpace(location))
            {
                await transaction.RollbackAsync(cancellationToken);
                return new BookingShareCreateResponse(BookingShareCreateOutcome.BookingNotEligible, null, null);
            }

            // BookingShare has no configured lifetime in the approved requirements. Its link remains useful through the service and expires at its scheduled end.
            var share = new BookingShare
            {
                BookingId = booking.Id,
                Token = token,
                CreatedAt = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc),
                ExpiresAt = scheduledEndUtc
            };
            _dbContext.BookingShares.Add(share);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new BookingShareCreateResponse(
                BookingShareCreateOutcome.Created,
                share.Token,
                share.ExpiresAt);
        }

        public async Task<SharedBookingResponse?> GetActiveAsync(
            string token,
            DateTime nowUtc,
            CancellationToken cancellationToken = default) =>
            await _dbContext.BookingShares.AsNoTracking()
                .Where(share => share.Token == token
                    && share.ExpiresAt > nowUtc
                    && share.Booking.Status == BookingStatus.Confirmed
                    && share.Booking.Payment.Status == PaymentStatus.Paid)
                .Select(share => new SharedBookingResponse(
                    share.Booking.BookingType,
                    share.Booking.BookingType == BookingType.Experience
                        ? share.Booking.Experience!.Title
                        : "Guide booking",
                    _dbContext.ApplicationUsers.Where(user => user.Id == share.Booking.ProviderUserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? "Local provider",
                    share.Booking.ScheduledStart,
                    share.Booking.ScheduledEnd,
                    share.Booking.BookingType == BookingType.Experience
                        ? share.Booking.Experience!.CulturalArea.Name
                        : null,
                    share.Booking.BookingType == BookingType.Experience
                        ? share.Booking.Experience!.ExactMeetingPoint
                        : share.Booking.Destination!))
                .SingleOrDefaultAsync(cancellationToken);
    }
}
