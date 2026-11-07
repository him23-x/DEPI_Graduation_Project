
namespace DiscoverEgypt.Application.ViewModels.Bookings
{
    public sealed record HostBookingDetailsViewModel
    {
        public required BookingDetailsViewModel Booking { get; init; }
        public string? Notice { get; init; }
        public string? Error { get; init; }
    }
}

