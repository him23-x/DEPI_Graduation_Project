using DiscoverEgypt.Application.DTOs.Bookings;
using MediatR;

using DiscoverEgypt.Application.Enums.Bookings;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Commands;

public sealed record CompleteHostBookingCommand(string UserId, int BookingId)
    : IRequest<HostBookingActionOutcome>;
