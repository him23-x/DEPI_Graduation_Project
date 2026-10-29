using DiscoverEgypt.Application.DTOs.Bookings;

namespace DiscoverEgypt.Presentation.ViewModels.Bookings;

public sealed class HostBookingDetailsViewModel
{
    public required BookingDetailsViewModel Booking { get; init; }
    public string? Notice { get; init; }
    public string? Error { get; init; }
}
