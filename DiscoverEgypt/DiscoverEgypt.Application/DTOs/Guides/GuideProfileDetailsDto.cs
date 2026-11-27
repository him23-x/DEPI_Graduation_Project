namespace DiscoverEgypt.Application.DTOs.Guides
{
    public sealed record GuideProfileDetailsDto(
        GuideProfileSummaryDto Profile,
        IReadOnlyList<GuideReviewDto> Reviews);
}
