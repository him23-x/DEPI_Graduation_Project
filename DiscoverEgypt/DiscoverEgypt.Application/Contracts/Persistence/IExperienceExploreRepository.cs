using DiscoverEgypt.Application.DTOs.Experiences;
using DiscoverEgypt.Application.DTOs.Recommendations;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IExperienceExploreRepository
    {
        Task<IReadOnlyList<ExploreExperienceDto>> GetPageAsync(
            string search,
            int? typeId,
            int? areaId,
            decimal? minPrice,
            decimal? maxPrice,
            string sort,
            int skip,
            int take,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<RecommendationExperienceCandidateDto>> GetRecommendationCandidatesAsync(
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            string search,
            int? typeId,
            int? areaId,
            decimal? minPrice,
            decimal? maxPrice,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ExperienceFilterOptionDto>> GetTypesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ExperienceFilterOptionDto>> GetAreasAsync(CancellationToken cancellationToken = default);
        Task<string?> GetPublishedImageKeyAsync(int experienceId, CancellationToken cancellationToken = default);
    }
}
