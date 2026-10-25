using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.Bookings;

public sealed class HostBookingsIndexDto
{
    public HostBookingTab SelectedTab { get; init; }
    public IReadOnlyList<HostBookingListItem> Bookings { get; init; } = [];
    public string? Notice { get; init; }
    public string? Error { get; init; }
}

