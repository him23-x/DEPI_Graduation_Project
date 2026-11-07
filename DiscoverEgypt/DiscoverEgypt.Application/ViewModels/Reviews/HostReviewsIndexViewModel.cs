namespace DiscoverEgypt.Application.ViewModels.Reviews
{
    public sealed record HostReviewsIndexViewModel(
        IReadOnlyList<HostReviewItemViewModel> Reviews,
        decimal AverageRating,
        int ReviewCount,
        int Page,
        int PageSize,
        int TotalCount)
    {
        public int PageCount => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    }
}

