using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.Bookings;

public sealed class HostBookingDetailsDto
{
    public required HostBookingDetails Booking { get; init; }
    public string? Notice { get; init; }
    public string? Error { get; init; }
}

