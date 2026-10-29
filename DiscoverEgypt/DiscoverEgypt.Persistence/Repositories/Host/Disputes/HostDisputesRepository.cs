using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Disputes;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host.Disputes;

public sealed class HostDisputesRepository(DiscoverEgyptDbContext dbContext) : IHostDisputesRepository
{
    public async Task<HostDisputesPageDto> GetForHostAsync(
        string userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var hostDisputes = ForHost(userId);
        var totalCount = await hostDisputes.CountAsync(cancellationToken);

        var items = await (
            from dispute in hostDisputes
            join guest in dbContext.ApplicationUsers.AsNoTracking()
                on dispute.Booking.TouristUserId equals guest.Id
            orderby dispute.CreatedAt descending, dispute.Id descending
            select new
            {
                dispute.Id,
                dispute.BookingId,
                ExperienceTitle = dispute.Booking.Experience!.Title,
                guest.FullName,
                guest.UserName,
                dispute.Booking.ScheduledStart,
                dispute.CreatedAt,
                dispute.Status,
                dispute.Outcome,
                PayoutStatus = (PayoutStatus?)dispute.Booking.Payout!.Status,
                PayoutAmount = (decimal?)dispute.Booking.Payout!.Amount,
                EligibleAt = (DateTime?)dispute.Booking.Payout!.EligibleAt,
                IsPayoutHeld = (dispute.Status == DisputeStatus.Open || dispute.Status == DisputeStatus.UnderReview)
                    && dispute.Booking.Payout != null
                    && dispute.Booking.Payout.Status == PayoutStatus.Pending
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var disputes = items.Select(item => new HostDisputeListItemDto(
            item.Id,
            item.BookingId,
            item.ExperienceTitle,
            GuestName(item.FullName, item.UserName),
            item.ScheduledStart,
            item.CreatedAt,
            item.Status,
            item.Outcome,
            item.PayoutStatus,
            item.PayoutAmount,
            item.EligibleAt,
            item.IsPayoutHeld)).ToArray();

        return new HostDisputesPageDto(disputes, page, pageSize, totalCount);
    }

    public async Task<HostDisputeDetailsDto?> GetDetailsAsync(
        string userId,
        int disputeId,
        CancellationToken cancellationToken = default)
    {
        var details = await (
            from dispute in ForHost(userId)
            join guest in dbContext.ApplicationUsers.AsNoTracking()
                on dispute.Booking.TouristUserId equals guest.Id
            where dispute.Id == disputeId
            select new
            {
                Dispute = dispute,
                guest.FullName,
                guest.UserName,
                dispute.Booking.Experience!.Title,
                dispute.Booking.ScheduledStart,
                dispute.Booking.ScheduledEnd,
                NumberOfGuests = dispute.Booking.NumberOfGuests ?? 0,
                dispute.Booking.ServiceAmount,
                dispute.Booking.Status,
                PayoutAmount = (decimal?)dispute.Booking.Payout!.Amount,
                PayoutStatus = (PayoutStatus?)dispute.Booking.Payout!.Status,
                EligibleAt = (DateTime?)dispute.Booking.Payout!.EligibleAt,
                PaidAt = (DateTime?)dispute.Booking.Payout!.PaidAt,
                IsPayoutHeld = (dispute.Status == DisputeStatus.Open || dispute.Status == DisputeStatus.UnderReview)
                    && dispute.Booking.Payout != null
                    && dispute.Booking.Payout.Status == PayoutStatus.Pending
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (details is null) return null;
        var disputeRecord = details.Dispute;
        return new HostDisputeDetailsDto(
            disputeRecord.Id,
            disputeRecord.BookingId,
            details.Title,
            GuestName(details.FullName, details.UserName),
            details.ScheduledStart,
            details.ScheduledEnd,
            details.NumberOfGuests,
            details.ServiceAmount,
            details.Status,
            disputeRecord.Status,
            disputeRecord.Outcome,
            disputeRecord.Description,
            disputeRecord.ResolutionNote,
            disputeRecord.CreatedAt,
            disputeRecord.ResolvedAt,
            details.PayoutAmount,
            details.PayoutStatus,
            details.EligibleAt,
            details.PaidAt,
            details.IsPayoutHeld);
    }

    private IQueryable<Dispute> ForHost(string userId) => dbContext.Disputes
        .AsNoTracking()
        .Where(dispute => dispute.Booking.ProviderUserId == userId
            && dispute.Booking.BookingType == BookingType.Experience
            && dispute.Booking.Experience != null
            && dispute.Booking.Experience.HostProfile.UserId == userId);

    private static string GuestName(string? fullName, string? userName) =>
        !string.IsNullOrWhiteSpace(fullName) ? fullName : userName ?? "Discover Egypt guest";
}
