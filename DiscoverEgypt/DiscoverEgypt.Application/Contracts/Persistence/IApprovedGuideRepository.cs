using DiscoverEgypt.Application.DTOs.Guides;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IApprovedGuideRepository
    {
        Task<IReadOnlyList<GuideProfileSummaryDto>> GetPageAsync(string search, int? languageId, int? culturalAreaId, int skip, int take, CancellationToken cancellationToken = default);
        Task<int> CountAsync(string search, int? languageId, int? culturalAreaId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<GuideDirectoryOptionDto>> GetLanguagesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<GuideDirectoryOptionDto>> GetCulturalAreasAsync(CancellationToken cancellationToken = default);
        Task<GuideProfileDetailsDto?> GetDetailsAsync(int guideProfileId, CancellationToken cancellationToken = default);
        Task<string?> GetApprovedProfileImageKeyAsync(int guideProfileId, CancellationToken cancellationToken = default);
    }
}
