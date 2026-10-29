namespace DiscoverEgypt.Application.DTOs.Reviews;

public sealed record HostReviewsPageDto(
    IReadOnlyList<HostReviewListItemDto> Reviews,
    decimal AverageRating,
    int ReviewCount,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int PageCount => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
}
