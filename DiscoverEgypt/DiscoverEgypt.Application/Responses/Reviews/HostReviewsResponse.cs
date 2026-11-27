namespace DiscoverEgypt.Application.Responses.Reviews
{
    public sealed record HostReviewsResponse(
        IReadOnlyList<HostReviewItemResponse> Reviews,
        decimal AverageRating,
        int ReviewCount,
        int Page,
        int PageSize,
        int TotalCount)
    {
        public int PageCount => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    }
}
