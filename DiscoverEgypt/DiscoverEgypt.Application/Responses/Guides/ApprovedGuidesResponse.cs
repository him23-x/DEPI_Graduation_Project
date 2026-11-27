using DiscoverEgypt.Application.DTOs.Guides;

namespace DiscoverEgypt.Application.Responses.Guides
{
    public sealed record ApprovedGuidesResponse(
        IReadOnlyList<GuideProfileSummaryDto> Items,
        IReadOnlyList<GuideDirectoryOptionDto> Languages,
        IReadOnlyList<GuideDirectoryOptionDto> CulturalAreas,
        string Search,
        int? LanguageId,
        int? CulturalAreaId,
        int Page,
        int PageSize,
        int TotalCount)
    {
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
