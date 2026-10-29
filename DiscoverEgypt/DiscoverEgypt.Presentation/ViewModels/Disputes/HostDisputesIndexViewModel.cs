namespace DiscoverEgypt.Presentation.ViewModels.Disputes;

public sealed class HostDisputesIndexViewModel
{
    public IReadOnlyList<HostDisputeListItemViewModel> Disputes { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; } = 10;
    public int TotalCount { get; init; }
    public int PageCount => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
}
