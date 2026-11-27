using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Responses.Accounts;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host.Disputes
{
    public sealed class HostDisputesRepository : IHostDisputesRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public HostDisputesRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Dispute>> GetForHostAsync(
            string userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var hostDisputes = ForHost(userId);
            return await hostDisputes
                .Include(dispute => dispute.Booking)
                    .ThenInclude(booking => booking.Experience)
                .Include(dispute => dispute.Booking.Payout)
                .OrderByDescending(dispute => dispute.CreatedAt)
                .ThenByDescending(dispute => dispute.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public Task<int> GetCountAsync(string userId, CancellationToken cancellationToken = default) => ForHost(userId).CountAsync(cancellationToken);

        public async Task<IReadOnlyDictionary<string, UserSummaryResponse>> GetTouristsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
            await _dbContext.ApplicationUsers.AsNoTracking()
                .Where(user => userIds.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id,
                    user => new UserSummaryResponse(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty, user.FullName),
                    cancellationToken);

        public async Task<Dispute?> GetDetailsAsync(
            string userId,
            int disputeId,
            CancellationToken cancellationToken = default)
        {
            return await ForHost(userId)
                .Include(dispute => dispute.Booking)
                    .ThenInclude(booking => booking.Experience)
                .Include(dispute => dispute.Booking.Payout)
                .SingleOrDefaultAsync(dispute => dispute.Id == disputeId, cancellationToken);
        }

        private IQueryable<Dispute> ForHost(string userId) => _dbContext.Disputes
            .AsNoTracking()
            .Where(dispute => dispute.Booking.ProviderUserId == userId
                && dispute.Booking.BookingType == BookingType.Experience
                && dispute.Booking.Experience != null
                && dispute.Booking.Experience.HostProfile.UserId == userId);

    }
}

