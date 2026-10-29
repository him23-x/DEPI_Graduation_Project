
namespace DiscoverEgypt.Application.DTOs.Bookings;

public sealed record HostBookingDetailsDto
{
    public required HostBookingDetailDto Booking { get; init; }
    public string? Notice { get; init; }
    public string? Error { get; init; }
}

