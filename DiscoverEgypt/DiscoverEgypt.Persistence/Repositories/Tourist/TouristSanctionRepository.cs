using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Domain.Enums.Sanctions;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Tourist
{
    public sealed class TouristSanctionRepository : ITouristSanctionRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public TouristSanctionRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public Task<bool> HasActiveTouristBanAsync(
            string touristUserId,
            DateTime nowUtc,
            CancellationToken cancellationToken = default) =>
            _dbContext.Sanctions.AsNoTracking().AnyAsync(sanction =>
                sanction.UserId == touristUserId
                && sanction.SanctionType == SanctionType.TouristBan
                && sanction.StartsAt <= nowUtc
                && sanction.EndsAt >= nowUtc,
                cancellationToken);
    }
}
