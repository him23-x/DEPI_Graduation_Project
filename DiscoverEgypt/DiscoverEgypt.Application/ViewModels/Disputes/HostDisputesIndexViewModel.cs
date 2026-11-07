namespace DiscoverEgypt.Application.ViewModels.Disputes
{
    public sealed record HostDisputesIndexViewModel(
        IReadOnlyList<HostDisputeListItemViewModel> Disputes,
        int Page,
        int PageSize,
        int TotalCount)
    {
        public int PageCount => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    }
}

