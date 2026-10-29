
using DiscoverEgypt.Application.Enums.Bookings;

namespace DiscoverEgypt.Application.DTOs.Bookings;

public sealed record HostBookingsIndexDto
{
    public HostBookingTab SelectedTab { get; init; }
    public IReadOnlyList<HostBookingListItemDto> Bookings { get; init; } = [];
    public string? Notice { get; init; }
    public string? Error { get; init; }
}

