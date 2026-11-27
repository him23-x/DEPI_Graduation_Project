using DiscoverEgypt.Application.Enums.Bookings;

namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record HostBookingsResponse
    {
        public HostBookingTab SelectedTab { get; init; }
        public IReadOnlyList<HostBookingListItemResponse> Bookings { get; init; } = [];
        public string? Notice { get; init; }
        public string? Error { get; init; }
    }
}
