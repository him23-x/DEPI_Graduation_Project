using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Commands;

public sealed class AcceptHostBookingCommandHandler(IHostBookingRepository repository)
    : IRequestHandler<AcceptHostBookingCommand, HostBookingActionResult>
{
    public Task<HostBookingActionResult> Handle(AcceptHostBookingCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.AcceptAsync(request.UserId, request.BookingId, DateTime.UtcNow, cancellationToken);
    }
}
