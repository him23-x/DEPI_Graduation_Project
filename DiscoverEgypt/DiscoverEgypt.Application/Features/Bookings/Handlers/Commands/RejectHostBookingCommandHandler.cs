using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using MediatR;

using DiscoverEgypt.Application.Enums.Bookings;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Commands;

public sealed class RejectHostBookingCommandHandler(IHostBookingRepository repository)
    : IRequestHandler<RejectHostBookingCommand, HostBookingActionOutcome>
{
    public Task<HostBookingActionOutcome> Handle(RejectHostBookingCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.RejectAsync(request.UserId, request.BookingId, DateTime.UtcNow, cancellationToken);
    }
}
