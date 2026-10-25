using DiscoverEgypt.Application.DTOs.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Commands;

public sealed record CompleteHostBookingCommand(string UserId, int BookingId)
    : IRequest<HostBookingActionResult>;
