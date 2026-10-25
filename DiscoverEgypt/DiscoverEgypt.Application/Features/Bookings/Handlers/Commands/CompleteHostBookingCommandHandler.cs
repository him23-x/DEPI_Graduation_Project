using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Commands;

public sealed class CompleteHostBookingCommandHandler(IHostBookingRepository repository)
    : IRequestHandler<CompleteHostBookingCommand, HostBookingActionResult>
{
    public Task<HostBookingActionResult> Handle(CompleteHostBookingCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.CompleteAsync(request.UserId, request.BookingId, DateTime.UtcNow, cancellationToken);
    }
}
