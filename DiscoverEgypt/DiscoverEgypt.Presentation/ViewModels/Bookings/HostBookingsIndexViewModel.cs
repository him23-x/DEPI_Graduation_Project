using DiscoverEgypt.Application.DTOs.Bookings;

using DiscoverEgypt.Application.Enums.Bookings;

namespace DiscoverEgypt.Presentation.ViewModels.Bookings;

public sealed class HostBookingsIndexViewModel
{
    public HostBookingTab SelectedTab { get; init; }
    public IReadOnlyList<HostBookingListItemViewModel> Bookings { get; init; } = [];
    public string? Notice { get; init; }
    public string? Error { get; init; }
}
