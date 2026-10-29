using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using MediatR;

using DiscoverEgypt.Application.Enums.Bookings;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Commands;

public sealed class CompleteHostBookingCommandHandler(IHostBookingRepository repository)
    : IRequestHandler<CompleteHostBookingCommand, HostBookingActionOutcome>
{
    public Task<HostBookingActionOutcome> Handle(CompleteHostBookingCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.CompleteAsync(request.UserId, request.BookingId, DateTime.UtcNow, cancellationToken);
    }
}
