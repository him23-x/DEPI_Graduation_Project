using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Preferences;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface ITouristPreferenceRepository
    {
        Task<TouristPreference?> GetForUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Interest>> GetInterestsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ExperienceType>> GetExperienceTypesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CulturalArea>> GetCulturalAreasAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Language>> GetLanguagesAsync(CancellationToken cancellationToken = default);
        Task<bool> AreLookupSelectionsValidAsync(
            IReadOnlyCollection<int> interestIds,
            IReadOnlyCollection<int> experienceTypeIds,
            IReadOnlyCollection<int> culturalAreaIds,
            IReadOnlyCollection<int> languageIds,
            CancellationToken cancellationToken = default);
        Task SaveAsync(
            string userId,
            Budget budget,
            DurationPreference duration,
            GuidePreference guidePreference,
            IReadOnlyCollection<int> interestIds,
            IReadOnlyCollection<int> experienceTypeIds,
            IReadOnlyCollection<int> culturalAreaIds,
            IReadOnlyCollection<int> languageIds,
            CancellationToken cancellationToken = default);
    }
}
