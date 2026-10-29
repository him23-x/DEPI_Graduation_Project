using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Presentation.ViewModels.Earnings;

public sealed class HostEarningsIndexViewModel
{
    public IReadOnlyList<HostPayoutItemViewModel> Payouts { get; init; } = [];
    public decimal PendingAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal CancelledAmount { get; init; }
    public decimal TotalEarnings => PendingAmount + PaidAmount;
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; } = 10;
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public PayoutStatus? SelectedStatus { get; init; }
}
