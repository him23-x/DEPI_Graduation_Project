using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Disputes;
using DiscoverEgypt.Application.Responses.Disputes;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;
using DiscoverEgypt.Domain.Enums.Sanctions;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Admin.Disputes
{
    public sealed class AdminDisputeReviewRepository : IAdminDisputeReviewRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public AdminDisputeReviewRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public async Task<IReadOnlyList<AdminDisputeQueueItemResponse>> GetOpenAsync(
            CancellationToken cancellationToken = default)
        {
            var items = await _dbContext.Disputes.AsNoTracking()
                .Where(dispute => dispute.Status == DisputeStatus.Open
                    || dispute.Status == DisputeStatus.UnderReview)
                .OrderBy(dispute => dispute.CreatedAt)
                .ThenBy(dispute => dispute.Id)
                .Select(dispute => new AdminDisputeQueueItemResponse(
                    dispute.Id,
                    dispute.BookingId,
                    dispute.Booking.BookingType == BookingType.Experience
                        ? dispute.Booking.Experience!.Title
                        : _dbContext.ApplicationUsers.Where(user => user.Id == dispute.Booking.ProviderUserId)
                            .Select(user => user.FullName).FirstOrDefault() ?? "Local guide",
                    dispute.Booking.BookingType,
                    _dbContext.ApplicationUsers.Where(user => user.Id == dispute.Booking.TouristUserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? "Tourist",
                    _dbContext.ApplicationUsers.Where(user => user.Id == dispute.Booking.ProviderUserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? "Provider",
                    dispute.Booking.Status,
                    dispute.Status,
                    dispute.CreatedAt))
                .ToListAsync(cancellationToken);

            return items.Select(item => item with
            {
                CreatedAt = DateTime.SpecifyKind(item.CreatedAt, DateTimeKind.Utc)
            }).ToArray();
        }

        public async Task<AdminDisputeDetailsResponse?> GetOpenDetailsAsync(
            int disputeId,
            CancellationToken cancellationToken = default)
        {
            var details = await _dbContext.Disputes.AsNoTracking()
                .Where(dispute => dispute.Id == disputeId
                    && (dispute.Status == DisputeStatus.Open || dispute.Status == DisputeStatus.UnderReview))
                .Select(dispute => new AdminDisputeDetailsResponse(
                    dispute.Id,
                    dispute.BookingId,
                    dispute.Booking.BookingType,
                    dispute.Booking.BookingType == BookingType.Experience
                        ? dispute.Booking.Experience!.Title
                        : _dbContext.ApplicationUsers.Where(user => user.Id == dispute.Booking.ProviderUserId)
                            .Select(user => user.FullName).FirstOrDefault() ?? "Local guide",
                    _dbContext.ApplicationUsers.Where(user => user.Id == dispute.Booking.TouristUserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? "Tourist",
                    _dbContext.ApplicationUsers.Where(user => user.Id == dispute.Booking.ProviderUserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? "Provider",
                    _dbContext.ApplicationUsers.Where(user => user.Id == dispute.OpenedByUserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? "Account holder",
                    dispute.Description,
                    dispute.Status,
                    dispute.Booking.Status,
                    dispute.Booking.ScheduledStart,
                    dispute.Booking.ScheduledEnd,
                    dispute.Booking.ServiceAmount,
                    dispute.Booking.PlatformFeeAmount,
                    dispute.Booking.TotalAmount,
                    dispute.Booking.Payment.Status,
                    dispute.Booking.Payout == null ? null : dispute.Booking.Payout.Amount,
                    dispute.Booking.Payout == null ? null : dispute.Booking.Payout.Status,
                    dispute.CreatedAt))
                .SingleOrDefaultAsync(cancellationToken);

            return details is null
                ? null
                : details with
                {
                    ScheduledStart = DateTime.SpecifyKind(details.ScheduledStart, DateTimeKind.Utc),
                    ScheduledEnd = DateTime.SpecifyKind(details.ScheduledEnd, DateTimeKind.Utc),
                    CreatedAt = DateTime.SpecifyKind(details.CreatedAt, DateTimeKind.Utc)
                };
        }

        public async Task<AdminDisputeResolutionOutcome> ResolveAsync(
            string adminUserId,
            int disputeId,
            DisputeOutcome outcome,
            string? resolutionNote,
            int? sanctionDays,
            DateTime resolvedAtUtc,
            CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

            var dispute = await _dbContext.Disputes
                .Include(item => item.Booking).ThenInclude(booking => booking.Payment)
                .Include(item => item.Booking).ThenInclude(booking => booking.Payout)
                .Include(item => item.Booking).ThenInclude(booking => booking.Refunds)
                .SingleOrDefaultAsync(item => item.Id == disputeId, cancellationToken);
            if (dispute is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AdminDisputeResolutionOutcome.DisputeUnavailable;
            }

            if (dispute.Status is not (DisputeStatus.Open or DisputeStatus.UnderReview))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AdminDisputeResolutionOutcome.DisputeAlreadyResolved;
            }

            var nowUtc = DateTime.SpecifyKind(resolvedAtUtc, DateTimeKind.Utc);
            dispute.Status = DisputeStatus.Resolved;
            dispute.Outcome = outcome;
            dispute.ResolutionNote = resolutionNote;
            dispute.ResolvedByAdminId = adminUserId;
            dispute.ResolvedAt = nowUtc;

            if (outcome == DisputeOutcome.ProviderFault)
            {
                AddProviderFaultRefund(dispute, adminUserId, nowUtc);
                if (dispute.Booking.Payout is not null)
                {
                    dispute.Booking.Payout.Amount = 0m;
                    dispute.Booking.Payout.Status = PayoutStatus.Cancelled;
                    dispute.Booking.Payout.PaidAt = null;
                }
            }

            if (outcome is DisputeOutcome.ProviderFault or DisputeOutcome.TouristFault)
                AddSanction(dispute, adminUserId, outcome, sanctionDays, nowUtc);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return AdminDisputeResolutionOutcome.Success;
        }

        private void AddProviderFaultRefund(Dispute dispute, string adminUserId, DateTime nowUtc)
        {
            var booking = dispute.Booking;
            var refunded = booking.Refunds.Sum(refund => refund.Amount);
            var remainingServiceAmount = Math.Max(0m, booking.ServiceAmount - refunded);
            if (remainingServiceAmount > 0m)
            {
                _dbContext.Refunds.Add(new Refund
                {
                    BookingId = booking.Id,
                    DisputeId = dispute.Id,
                    Amount = remainingServiceAmount,
                    Reason = "Service amount refunded after the Admin resolved the dispute as provider fault.",
                    DecidedByAdminId = adminUserId,
                    CreatedAt = nowUtc
                });
            }

            var totalRefunded = refunded + remainingServiceAmount;
            if (totalRefunded > 0m)
                booking.Payment.Status = totalRefunded >= booking.Payment.Amount
                    ? PaymentStatus.Refunded
                    : PaymentStatus.PartiallyRefunded;
        }

        private void AddSanction(
            Dispute dispute,
            string adminUserId,
            DisputeOutcome outcome,
            int? sanctionDays,
            DateTime startsAtUtc)
        {
            if (!sanctionDays.HasValue || sanctionDays.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(sanctionDays), "Sanction duration must be positive.");

            var providerFault = outcome == DisputeOutcome.ProviderFault;
            _dbContext.Sanctions.Add(new Sanction
            {
                UserId = providerFault ? dispute.Booking.ProviderUserId : dispute.Booking.TouristUserId,
                DisputeId = dispute.Id,
                SanctionType = providerFault ? SanctionType.ProviderSuspension : SanctionType.TouristBan,
                Days = sanctionDays.Value,
                StartsAt = startsAtUtc,
                EndsAt = startsAtUtc.AddDays(sanctionDays.Value),
                DecidedByAdminId = adminUserId
            });
        }
    }
}
