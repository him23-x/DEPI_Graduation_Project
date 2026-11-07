
using DiscoverEgypt.Application.Enums.Bookings;

namespace DiscoverEgypt.Application.ViewModels.Bookings
{
    public sealed record HostBookingsIndexViewModel
    {
        public HostBookingTab SelectedTab { get; init; }
        public IReadOnlyList<HostBookingListItemViewModel> Bookings { get; init; } = [];
        public string? Notice { get; init; }
        public string? Error { get; init; }
    }
}

