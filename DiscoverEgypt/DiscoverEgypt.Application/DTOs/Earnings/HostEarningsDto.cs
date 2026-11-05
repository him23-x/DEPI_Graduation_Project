using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.DTOs.Earnings;

public sealed record HostEarningsDto(
    IReadOnlyList<HostPayoutListItemDto> Payouts,
    decimal PendingAmount,
    decimal PaidAmount,
    decimal CancelledAmount,
    int TotalCount,
    int Page,
    int PageSize)
{
    public PayoutStatus? SelectedStatus { get; init; }
    public decimal TotalEarnings => PendingAmount + PaidAmount;
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
}
