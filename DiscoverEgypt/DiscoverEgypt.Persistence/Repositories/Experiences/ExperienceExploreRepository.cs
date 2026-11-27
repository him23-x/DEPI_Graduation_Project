using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Experiences;
using DiscoverEgypt.Application.DTOs.Recommendations;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Experiences;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Experiences
{
    public sealed class ExperienceExploreRepository : IExperienceExploreRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public ExperienceExploreRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<ExploreExperienceDto>> GetPageAsync(
            string search,
            int? typeId,
            int? areaId,
            decimal? minPrice,
            decimal? maxPrice,
            string sort,
            int skip,
            int take,
            CancellationToken cancellationToken = default)
        {
            var query = Filtered(search, typeId, areaId, minPrice, maxPrice);
            query = sort switch
            {
                "price-low" => query.OrderBy(experience => experience.PricePerPerson).ThenBy(experience => experience.Id),
                "price-high" => query.OrderByDescending(experience => experience.PricePerPerson).ThenBy(experience => experience.Id),
                "rating" => query.OrderByDescending(experience => experience.Bookings
                    .Where(booking => booking.Review != null)
                    .Select(booking => (decimal?)booking.Review!.Rating)
                    .Average() ?? 0m).ThenByDescending(experience => experience.Id),
                _ => query.OrderByDescending(experience => experience.CreatedAt).ThenByDescending(experience => experience.Id)
            };

            return await query
                .Skip(skip)
                .Take(take)
                .Select(experience => new ExploreExperienceDto(
                    experience.Id,
                    experience.Title,
                    experience.ExperienceType.Name,
                    experience.CulturalArea.Name,
                    _dbContext.ApplicationUsers
                        .Where(user => user.Id == experience.HostProfile.UserId)
                        .Select(user => user.FullName)
                        .FirstOrDefault() ?? string.Empty,
                    experience.Media
                        .Where(media => media.MediaType == MediaType.Image)
                        .OrderByDescending(media => media.IsCover)
                        .ThenBy(media => media.DisplayOrder)
                        .Select(media => media.StorageKey)
                        .FirstOrDefault(),
                    experience.CulturalStory,
                    experience.PricePerPerson,
                    experience.DurationMinutes,
                    experience.Bookings
                        .Where(booking => booking.Review != null)
                        .Select(booking => (decimal?)booking.Review!.Rating)
                        .Average() ?? 0m,
                    experience.Bookings.Count(booking => booking.Review != null)))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<RecommendationExperienceCandidateDto>> GetRecommendationCandidatesAsync(
            CancellationToken cancellationToken = default)
        {
            var experiences = await _dbContext.Experiences.AsNoTracking()
                .Where(experience => experience.Status == ExperienceStatus.Published)
                .Include(experience => experience.ExperienceType)
                .Include(experience => experience.CulturalArea)
                .Include(experience => experience.HostProfile)
                .Include(experience => experience.Media.Where(media => media.MediaType == MediaType.Image))
                .Include(experience => experience.ExperienceInterests)
                .Include(experience => experience.ExperienceLanguages)
                .Include(experience => experience.Bookings)
                    .ThenInclude(booking => booking.Review)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            var hostUserIds = experiences.Select(experience => experience.HostProfile.UserId).Distinct().ToArray();
            var hostNames = await _dbContext.ApplicationUsers.AsNoTracking()
                .Where(user => hostUserIds.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id, user => user.FullName, cancellationToken);

            return experiences.Select(experience =>
            {
                var ratings = experience.Bookings
                    .Where(booking => booking.Review is not null)
                    .Select(booking => booking.Review!.Rating)
                    .ToArray();
                var cover = experience.Media
                    .OrderByDescending(media => media.IsCover)
                    .ThenBy(media => media.DisplayOrder)
                    .FirstOrDefault();

                return new RecommendationExperienceCandidateDto(
                    experience.Id,
                    experience.Status,
                    experience.Title,
                    experience.ExperienceType.Name,
                    experience.ExperienceTypeId,
                    experience.CulturalArea.Name,
                    experience.CulturalAreaId,
                    hostNames.GetValueOrDefault(experience.HostProfile.UserId) ?? string.Empty,
                    cover?.StorageKey,
                    experience.CulturalStory,
                    experience.PricePerPerson,
                    experience.DurationMinutes,
                    ratings.Length == 0 ? 0m : (decimal)ratings.Average(),
                    ratings.Length,
                    experience.ExperienceInterests.Select(link => link.InterestId).Distinct().ToArray(),
                    experience.ExperienceLanguages.Select(link => link.LanguageId).Distinct().ToArray());
            }).ToArray();
        }

        public Task<int> CountAsync(
            string search,
            int? typeId,
            int? areaId,
            decimal? minPrice,
            decimal? maxPrice,
            CancellationToken cancellationToken = default) =>
            Filtered(search, typeId, areaId, minPrice, maxPrice).CountAsync(cancellationToken);

        public async Task<IReadOnlyList<ExperienceFilterOptionDto>> GetTypesAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.ExperienceTypes.AsNoTracking()
                .Where(type => type.Experiences.Any(experience => experience.Status == ExperienceStatus.Published))
                .OrderBy(type => type.Name)
                .Select(type => new ExperienceFilterOptionDto(type.Id, type.Name))
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<ExperienceFilterOptionDto>> GetAreasAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.CulturalAreas.AsNoTracking()
                .Where(area => area.Experiences.Any(experience => experience.Status == ExperienceStatus.Published))
                .OrderBy(area => area.Name)
                .Select(area => new ExperienceFilterOptionDto(area.Id, area.Name))
                .ToListAsync(cancellationToken);

        public Task<string?> GetPublishedImageKeyAsync(int experienceId, CancellationToken cancellationToken = default) =>
            _dbContext.ExperienceMedia.AsNoTracking()
                .Where(media => media.ExperienceId == experienceId
                    && media.MediaType == MediaType.Image
                    && media.Experience.Status == ExperienceStatus.Published)
                .OrderByDescending(media => media.IsCover)
                .ThenBy(media => media.DisplayOrder)
                .Select(media => media.StorageKey)
                .FirstOrDefaultAsync(cancellationToken);

        private IQueryable<Experience> Filtered(
            string search,
            int? typeId,
            int? areaId,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var query = _dbContext.Experiences.AsNoTracking()
                .Where(experience => experience.Status == ExperienceStatus.Published);

            if (typeId.HasValue)
                query = query.Where(experience => experience.ExperienceTypeId == typeId.Value);
            if (areaId.HasValue)
                query = query.Where(experience => experience.CulturalAreaId == areaId.Value);
            if (minPrice.HasValue)
                query = query.Where(experience => experience.PricePerPerson >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(experience => experience.PricePerPerson <= maxPrice.Value);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(experience => experience.Title.Contains(search)
                    || experience.Description.Contains(search)
                    || experience.CulturalStory.Contains(search)
                    || experience.ExperienceType.Name.Contains(search)
                    || experience.CulturalArea.Name.Contains(search));
            }

            return query;
        }
    }
}
