using DiscoverEgypt.Domain.Entities;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IHostExperienceRepository
    {
        Task<IReadOnlyList<Experience>> GetForHostAsync(
            string userId,
            ExperienceStatus? status,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ExperienceType>> GetExperienceTypesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CulturalArea>> GetCulturalAreasAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Interest>> GetInterestsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Language>> GetLanguagesAsync(CancellationToken cancellationToken = default);

        Task<Experience?> GetEditableDraftAsync(string userId, int experienceId, CancellationToken cancellationToken = default);

        Task<int?> CreateDraftAsync(
            string userId,
            Experience experience,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateDraftAsync(string userId, int experienceId, Experience experience, CancellationToken cancellationToken = default);
    }
}

