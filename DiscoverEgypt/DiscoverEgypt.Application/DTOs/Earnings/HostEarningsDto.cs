namespace DiscoverEgypt.Application.DTOs.Earnings;

public sealed record HostEarningsDto(
    IReadOnlyList<HostPayoutListItemDto> Payouts,
    decimal PendingAmount,
    decimal PaidAmount,
    decimal CancelledAmount,
    int TotalCount,
    int Page,
    int PageSize);
