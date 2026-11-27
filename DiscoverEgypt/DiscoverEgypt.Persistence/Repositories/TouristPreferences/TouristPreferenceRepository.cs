using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Preferences;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.TouristPreferences
{
    public sealed class TouristPreferenceRepository : ITouristPreferenceRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public TouristPreferenceRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public Task<TouristPreference?> GetForUserAsync(string userId, CancellationToken cancellationToken = default) =>
            _dbContext.TouristPreferences.AsNoTracking()
                .Include(preference => preference.PreferenceInterests)
                .Include(preference => preference.PreferenceExperienceTypes)
                .Include(preference => preference.PreferenceCulturalAreas)
                .Include(preference => preference.PreferenceLanguages)
                    .ThenInclude(link => link.Language)
                .SingleOrDefaultAsync(preference => preference.UserId == userId, cancellationToken);

        public async Task<IReadOnlyList<Interest>> GetInterestsAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.Interests.AsNoTracking().OrderBy(item => item.Name).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<ExperienceType>> GetExperienceTypesAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.ExperienceTypes.AsNoTracking().OrderBy(item => item.Name).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<CulturalArea>> GetCulturalAreasAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.CulturalAreas.AsNoTracking().OrderBy(item => item.Name).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Language>> GetLanguagesAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.Languages.AsNoTracking().OrderBy(item => item.Name).ToListAsync(cancellationToken);

        public async Task<bool> AreLookupSelectionsValidAsync(
            IReadOnlyCollection<int> interestIds,
            IReadOnlyCollection<int> experienceTypeIds,
            IReadOnlyCollection<int> culturalAreaIds,
            IReadOnlyCollection<int> languageIds,
            CancellationToken cancellationToken = default)
        {
            return await ContainsAllAsync(_dbContext.Interests, interestIds, cancellationToken)
                && await ContainsAllAsync(_dbContext.ExperienceTypes, experienceTypeIds, cancellationToken)
                && await ContainsAllAsync(_dbContext.CulturalAreas, culturalAreaIds, cancellationToken)
                && await ContainsAllAsync(_dbContext.Languages, languageIds, cancellationToken);
        }

        public async Task SaveAsync(
            string userId,
            Budget budget,
            DurationPreference duration,
            GuidePreference guidePreference,
            IReadOnlyCollection<int> interestIds,
            IReadOnlyCollection<int> experienceTypeIds,
            IReadOnlyCollection<int> culturalAreaIds,
            IReadOnlyCollection<int> languageIds,
            CancellationToken cancellationToken = default)
        {
            var preference = await _dbContext.TouristPreferences
                .Include(item => item.PreferenceInterests)
                .Include(item => item.PreferenceExperienceTypes)
                .Include(item => item.PreferenceCulturalAreas)
                .Include(item => item.PreferenceLanguages)
                .SingleOrDefaultAsync(item => item.UserId == userId, cancellationToken);

            if (preference is null)
            {
                preference = new TouristPreference { UserId = userId };
                _dbContext.TouristPreferences.Add(preference);
            }

            preference.Budget = budget;
            preference.Duration = duration;
            preference.GuidePreference = guidePreference;

            _dbContext.PreferenceInterests.RemoveRange(preference.PreferenceInterests);
            _dbContext.PreferenceExperienceTypes.RemoveRange(preference.PreferenceExperienceTypes);
            _dbContext.PreferenceCulturalAreas.RemoveRange(preference.PreferenceCulturalAreas);
            _dbContext.PreferenceLanguages.RemoveRange(preference.PreferenceLanguages);

            preference.PreferenceInterests = interestIds.Distinct().Select(id => new PreferenceInterest { InterestId = id }).ToList();
            preference.PreferenceExperienceTypes = experienceTypeIds.Distinct().Select(id => new PreferenceExperienceType { ExperienceTypeId = id }).ToList();
            preference.PreferenceCulturalAreas = culturalAreaIds.Distinct().Select(id => new PreferenceCulturalArea { CulturalAreaId = id }).ToList();
            preference.PreferenceLanguages = languageIds.Distinct().Select(id => new PreferenceLanguage { LanguageId = id }).ToList();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private static async Task<bool> ContainsAllAsync<TEntity>(
            DbSet<TEntity> set,
            IReadOnlyCollection<int> ids,
            CancellationToken cancellationToken) where TEntity : BaseEntity
        {
            if (ids.Count == 0) return true;
            var distinctIds = ids.Distinct().ToArray();
            return await set.CountAsync(item => distinctIds.Contains(item.Id), cancellationToken) == distinctIds.Length;
        }
    }
}
