namespace DiscoverEgypt.Application.DTOs.Disputes;

public sealed record HostDisputesPageDto(
    IReadOnlyList<HostDisputeListItemDto> Disputes,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int PageCount => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
}
