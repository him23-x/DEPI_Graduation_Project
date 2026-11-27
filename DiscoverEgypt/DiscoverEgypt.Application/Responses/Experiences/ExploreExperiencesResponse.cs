using DiscoverEgypt.Application.DTOs.Experiences;

namespace DiscoverEgypt.Application.Responses.Experiences
{
    public sealed record ExploreExperiencesResponse(
        IReadOnlyList<ExploreExperienceDto> Items,
        IReadOnlyList<ExperienceFilterOptionDto> Types,
        IReadOnlyList<ExperienceFilterOptionDto> Areas,
        int Page,
        int PageSize,
        int TotalCount,
        string Search,
        int? TypeId,
        int? AreaId,
        decimal? MinPrice,
        decimal? MaxPrice,
        string Sort)
    {
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
