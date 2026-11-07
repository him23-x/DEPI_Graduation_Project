using System.Data;
using System.Data.Common;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using DiscoverEgypt.Application.Enums.Bookings;

using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;
using DiscoverEgypt.Domain.Enums.Sanctions;

namespace DiscoverEgypt.Persistence.Repositories.Host.Bookings
{
    public sealed class HostBookingRepository : IHostBookingRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;
        public HostBookingRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Booking>> GetForHostAsync(
            string userId,
            HostBookingTab tab,
            CancellationToken cancellationToken = default)
        {
            var status = ToStatus(tab);
            var bookings = await HostBookings(userId)
                .AsNoTracking()
                .Include(booking => booking.Payment)
                .Include(booking => booking.Experience)
                .Include(booking => booking.Refunds)
                .Where(booking => booking.Payment.Status == PaymentStatus.Paid
                    || (tab == HostBookingTab.RejectedCancelled
                        && (booking.Status == BookingStatus.Rejected || booking.Status == BookingStatus.Cancelled)
                        && booking.Payment.Status == PaymentStatus.Refunded
                        && booking.Refunds.Any()))
                .Where(booking => (tab == HostBookingTab.RejectedCancelled
                    ? booking.Status == BookingStatus.Rejected || booking.Status == BookingStatus.Cancelled
                    : booking.Status == status))
                .OrderBy(booking => booking.ScheduledStart)
                .ThenBy(booking => booking.Id)
                .ToListAsync(cancellationToken);

            return bookings;
        }

        public async Task<Booking?> GetDetailsAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken = default)
        {
            var booking = await HostBookings(userId)
                .AsNoTracking()
                .Include(item => item.Payment)
                .Include(item => item.Experience)
                .Include(item => item.Payout)
                .Include(item => item.Disputes)
                .Include(item => item.Refunds)
                .Where(item => item.Payment.Status == PaymentStatus.Paid
                    || ((item.Status == BookingStatus.Rejected || item.Status == BookingStatus.Cancelled)
                        && item.Payment.Status == PaymentStatus.Refunded
                        && item.Refunds.Any()))
                .SingleOrDefaultAsync(item => item.Id == bookingId, cancellationToken);
            return booking;
        }

        public async Task<IReadOnlyDictionary<string, ApplicationUser>> GetTouristsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
            await _dbContext.ApplicationUsers.AsNoTracking().Where(user => userIds.Contains(user.Id)).ToDictionaryAsync(user => user.Id, cancellationToken);

        public async Task<bool> IsProviderSuspendedAsync(string userId, DateTime now, CancellationToken cancellationToken = default) =>
            await _dbContext.Sanctions.AnyAsync(sanction => sanction.UserId == userId
                && sanction.SanctionType == SanctionType.ProviderSuspension
                && sanction.StartsAt <= now
                && sanction.EndsAt >= now, cancellationToken);

        public async Task<bool> HasConfirmedOverlapAsync(string userId, DateTime scheduledStart, DateTime scheduledEnd, int exceptBookingId, CancellationToken cancellationToken = default) =>
            await _dbContext.Bookings.AnyAsync(booking => booking.Id != exceptBookingId
                && booking.ProviderUserId == userId
                && booking.BookingType == BookingType.Experience
                && booking.Status == BookingStatus.Confirmed
                && booking.ScheduledStart < scheduledEnd
                && booking.ScheduledEnd > scheduledStart, cancellationToken);

        public async Task<HostBookingActionOutcome> ExecuteActionAsync(
            string userId,
            int bookingId,
            Func<HostBookingActionContext, CancellationToken, Task<HostBookingActionOutcome>> action,
            CancellationToken cancellationToken = default)
        {
            var hostProfileId = await GetHostProfileIdAsync(userId, cancellationToken);
            if (!hostProfileId.HasValue)
            {
                return HostBookingActionOutcome.BookingUnavailable;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            if (!await AcquireHostBookingLockAsync(hostProfileId.Value, cancellationToken))
            {
                return HostBookingActionOutcome.ConcurrencyUnavailable;
            }

            var booking = await HostBookings(userId)
                .Include(item => item.Payment)
                .Include(item => item.Experience)
                .Include(item => item.Payout)
                .Include(item => item.Disputes)
                .SingleOrDefaultAsync(item => item.Id == bookingId, cancellationToken);
            if (booking is null)
            {
                return HostBookingActionOutcome.BookingUnavailable;
            }

            var now = DateTime.UtcNow;
            var context = new HostBookingActionContext
            {
                Booking = booking,
                Now = now,
                IsProviderSuspended = await IsProviderSuspendedAsync(userId, now, cancellationToken),
                HasOverlap = await HasConfirmedOverlapAsync(userId, booking.ScheduledStart, booking.ScheduledEnd, booking.Id, cancellationToken),
                HasOpenDispute = booking.Disputes.Any(dispute => dispute.Status is DisputeStatus.Open or DisputeStatus.UnderReview),
                HasPayout = booking.Payout is not null,
                AddPayout = payout => _dbContext.Payouts.Add(payout),
                AddRefund = refund => _dbContext.Refunds.Add(refund)
            };

            var result = await action(context, cancellationToken);
            if (result != HostBookingActionOutcome.Success)
            {
                return result;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return HostBookingActionOutcome.Success;
        }
        private IQueryable<Booking> HostBookings(string userId) => _dbContext.Bookings
            .Where(booking => booking.ProviderUserId == userId
                && booking.BookingType == BookingType.Experience
                && booking.Experience != null
                && booking.Experience.HostProfile.UserId == userId);

        private Task<int?> GetHostProfileIdAsync(string userId, CancellationToken cancellationToken) => _dbContext.HostProfiles
            .Where(profile => profile.UserId == userId)
            .Select(profile => (int?)profile.Id)
            .SingleOrDefaultAsync(cancellationToken);

        private async Task<bool> AcquireHostBookingLockAsync(int hostProfileId, CancellationToken cancellationToken)
        {
            var command = _dbContext.Database.GetDbConnection().CreateCommand();
            await using (command)
            {
                command.Transaction = _dbContext.Database.CurrentTransaction!.GetDbTransaction();
                command.CommandText = "DECLARE @lockResult int; EXEC @lockResult = sys.sp_getapplock @Resource = @resource, @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 10000; SELECT @lockResult;";
                var resource = command.CreateParameter();
                resource.ParameterName = "@resource";
                resource.Value = $"DiscoverEgypt:HostExperienceBookingAccept:{hostProfileId}";
                command.Parameters.Add(resource);
                var result = await command.ExecuteScalarAsync(cancellationToken);
                return result is not null && Convert.ToInt32(result) >= 0;
            }
        }

        private static BookingStatus ToStatus(HostBookingTab tab) => tab switch
        {
            HostBookingTab.Pending => BookingStatus.Pending,
            HostBookingTab.Confirmed => BookingStatus.Confirmed,
            HostBookingTab.Completed => BookingStatus.Completed,
            _ => BookingStatus.Rejected
        };
    }
}

