using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Disputes;
using DiscoverEgypt.Application.Features.Disputes.Policies;
using DiscoverEgypt.Application.Responses.Disputes;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Tourist
{
    public sealed class TouristDisputeRepository : ITouristDisputeRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public TouristDisputeRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public async Task<TouristDisputeBookingResponse?> GetOwnedBookingAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken = default)
        {
            var booking = await _dbContext.Bookings.AsNoTracking()
                .Where(item => item.Id == bookingId && item.TouristUserId == userId)
                .Select(item => new TouristDisputeBookingResponse(
                    item.Id,
                    item.BookingType,
                    item.BookingType == BookingType.Experience
                        ? item.Experience!.Title
                        : _dbContext.ApplicationUsers.Where(user => user.Id == item.ProviderUserId)
                            .Select(user => user.FullName).FirstOrDefault() ?? "Local guide",
                    item.ScheduledStart,
                    item.ScheduledEnd,
                    item.Status,
                    item.Payout == null ? null : item.Payout.Amount))
                .SingleOrDefaultAsync(cancellationToken);

            return booking is null
                ? null
                : booking with
                {
                    ScheduledStartUtc = DateTime.SpecifyKind(booking.ScheduledStartUtc, DateTimeKind.Utc),
                    ScheduledEndUtc = DateTime.SpecifyKind(booking.ScheduledEndUtc, DateTimeKind.Utc)
                };
        }

        public async Task<TouristDisputeOpenResponse> OpenAsync(
            string userId,
            int bookingId,
            string description,
            DateTime nowUtc,
            CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var booking = await _dbContext.Bookings
                .Include(item => item.Payout)
                .SingleOrDefaultAsync(item => item.Id == bookingId && item.TouristUserId == userId, cancellationToken);
            if (booking is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return new TouristDisputeOpenResponse(DisputeOpenOutcome.BookingUnavailable, null);
            }

            var scheduledEndUtc = DateTime.SpecifyKind(booking.ScheduledEnd, DateTimeKind.Utc);
            var providerAllocation = booking.Payout?.Amount;
            if (!TouristDisputeEligibilityPolicy.CanOpen(booking.Status, scheduledEndUtc, providerAllocation, nowUtc))
            {
                await transaction.RollbackAsync(cancellationToken);
                return new TouristDisputeOpenResponse(DisputeOpenOutcome.BookingNotEligible, null);
            }

            var dispute = new Dispute
            {
                BookingId = booking.Id,
                OpenedByUserId = userId,
                Description = description,
                Status = DisputeStatus.Open,
                CreatedAt = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc)
            };
            _dbContext.Disputes.Add(dispute);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new TouristDisputeOpenResponse(DisputeOpenOutcome.Created, dispute.Id);
        }

        public async Task<IReadOnlyList<TouristDisputeListItemResponse>> GetForTouristAsync(
            string userId,
            CancellationToken cancellationToken = default) =>
            await _dbContext.Disputes.AsNoTracking()
                .Where(dispute => dispute.OpenedByUserId == userId && dispute.Booking.TouristUserId == userId)
                .OrderByDescending(dispute => dispute.CreatedAt)
                .ThenByDescending(dispute => dispute.Id)
                .Select(dispute => new TouristDisputeListItemResponse(
                    dispute.Id,
                    dispute.BookingId,
                    dispute.Booking.BookingType == BookingType.Experience
                        ? dispute.Booking.Experience!.Title
                        : _dbContext.ApplicationUsers.Where(user => user.Id == dispute.Booking.ProviderUserId)
                            .Select(user => user.FullName).FirstOrDefault() ?? "Local guide",
                    dispute.Booking.ScheduledStart,
                    dispute.CreatedAt,
                    dispute.Status))
                .ToListAsync(cancellationToken);

        public async Task<TouristDisputeDetailsResponse?> GetDetailsForTouristAsync(
            string userId,
            int disputeId,
            CancellationToken cancellationToken = default)
        {
            var details = await _dbContext.Disputes.AsNoTracking()
                .Where(dispute => dispute.Id == disputeId
                    && dispute.OpenedByUserId == userId
                    && dispute.Booking.TouristUserId == userId)
                .Select(dispute => new TouristDisputeDetailsResponse(
                    dispute.Id,
                    dispute.BookingId,
                    dispute.Booking.BookingType,
                    dispute.Booking.BookingType == BookingType.Experience
                        ? dispute.Booking.Experience!.Title
                        : _dbContext.ApplicationUsers.Where(user => user.Id == dispute.Booking.ProviderUserId)
                            .Select(user => user.FullName).FirstOrDefault() ?? "Local guide",
                    dispute.Booking.ScheduledStart,
                    dispute.Booking.ScheduledEnd,
                    dispute.Booking.Status,
                    dispute.Description,
                    dispute.Status,
                    dispute.Outcome,
                    dispute.ResolutionNote,
                    dispute.CreatedAt,
                    dispute.ResolvedAt))
                .SingleOrDefaultAsync(cancellationToken);

            return details is null
                ? null
                : details with
                {
                    ScheduledStartUtc = DateTime.SpecifyKind(details.ScheduledStartUtc, DateTimeKind.Utc),
                    ScheduledEndUtc = DateTime.SpecifyKind(details.ScheduledEndUtc, DateTimeKind.Utc),
                    CreatedAtUtc = DateTime.SpecifyKind(details.CreatedAtUtc, DateTimeKind.Utc),
                    ResolvedAtUtc = details.ResolvedAtUtc.HasValue
                        ? DateTime.SpecifyKind(details.ResolvedAtUtc.Value, DateTimeKind.Utc)
                        : null
                };
        }
    }
}
