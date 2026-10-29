namespace DiscoverEgypt.Presentation.ViewModels.Reviews;

public sealed class HostReviewsIndexViewModel
{
    public IReadOnlyList<HostReviewItemViewModel> Reviews { get; init; } = [];
    public decimal AverageRating { get; init; }
    public int ReviewCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; } = 10;
    public int TotalCount { get; init; }
    public int PageCount => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
}
