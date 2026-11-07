using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.ViewModels.Earnings
{
    public sealed record HostEarningsIndexViewModel(
        IReadOnlyList<HostPayoutItemViewModel> Payouts,
        decimal PendingAmount,
        decimal PaidAmount,
        decimal CancelledAmount,
        int TotalCount,
        int Page,
        int PageSize)
    {
        public decimal TotalEarnings => PendingAmount + PaidAmount;
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
        public PayoutStatus? SelectedStatus { get; init; }
    }
}

