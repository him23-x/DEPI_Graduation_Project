namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface ITouristSanctionRepository
    {
        Task<bool> HasActiveTouristBanAsync(
            string touristUserId,
            DateTime nowUtc,
            CancellationToken cancellationToken = default);
    }
}
