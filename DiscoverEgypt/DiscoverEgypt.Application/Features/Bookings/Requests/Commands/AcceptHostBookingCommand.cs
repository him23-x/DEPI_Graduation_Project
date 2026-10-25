using DiscoverEgypt.Application.DTOs.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Commands;

public sealed record AcceptHostBookingCommand(string UserId, int BookingId)
    : IRequest<HostBookingActionResult>;
