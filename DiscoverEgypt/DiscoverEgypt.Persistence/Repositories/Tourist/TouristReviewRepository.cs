using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Reviews;
using DiscoverEgypt.Application.Features.Reviews.Policies;
using DiscoverEgypt.Application.Responses.Reviews;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Tourist
{
    public sealed class TouristReviewRepository : ITouristReviewRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public TouristReviewRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public async Task<TouristReviewBookingResponse?> GetOwnedBookingAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken = default) =>
            await _dbContext.Bookings.AsNoTracking()
                .Where(booking => booking.Id == bookingId && booking.TouristUserId == userId)
                .Select(booking => new TouristReviewBookingResponse(
                    booking.Id,
                    booking.BookingType == BookingType.Experience
                        ? booking.Experience!.Title
                        : _dbContext.ApplicationUsers.Where(user => user.Id == booking.ProviderUserId)
                            .Select(user => user.FullName).FirstOrDefault() ?? "Local guide",
                    booking.Status,
                    booking.Review != null))
                .SingleOrDefaultAsync(cancellationToken);

        public async Task<TouristReviewCreateResponse> CreateAsync(
            string userId,
            int bookingId,
            int rating,
            string? comment,
            DateTime nowUtc,
            CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

            var booking = await _dbContext.Bookings
                .Include(item => item.Review)
                .SingleOrDefaultAsync(item => item.Id == bookingId && item.TouristUserId == userId, cancellationToken);
            if (booking is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return new TouristReviewCreateResponse(TouristReviewCreateOutcome.BookingUnavailable, bookingId);
            }

            if (booking.Status != BookingStatus.Completed)
            {
                await transaction.RollbackAsync(cancellationToken);
                return new TouristReviewCreateResponse(TouristReviewCreateOutcome.BookingNotCompleted, bookingId);
            }

            if (!TouristReviewEligibilityPolicy.CanReview(booking.Status, booking.Review is not null))
            {
                await transaction.RollbackAsync(cancellationToken);
                return new TouristReviewCreateResponse(TouristReviewCreateOutcome.AlreadyReviewed, bookingId);
            }

            _dbContext.Reviews.Add(new Review
            {
                BookingId = booking.Id,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc)
            });
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync(cancellationToken);
                var reviewExists = await _dbContext.Reviews.AsNoTracking()
                    .AnyAsync(review => review.BookingId == bookingId, cancellationToken);
                if (reviewExists)
                    return new TouristReviewCreateResponse(TouristReviewCreateOutcome.AlreadyReviewed, bookingId);
                throw;
            }
            await transaction.CommitAsync(cancellationToken);

            return new TouristReviewCreateResponse(TouristReviewCreateOutcome.Created, bookingId);
        }
    }
}
