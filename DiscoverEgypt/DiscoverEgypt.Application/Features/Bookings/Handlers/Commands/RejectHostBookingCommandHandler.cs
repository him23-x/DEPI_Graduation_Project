using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Commands;

public sealed class RejectHostBookingCommandHandler(IHostBookingRepository repository)
    : IRequestHandler<RejectHostBookingCommand, HostBookingActionResult>
{
    public Task<HostBookingActionResult> Handle(RejectHostBookingCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.RejectAsync(request.UserId, request.BookingId, DateTime.UtcNow, cancellationToken);
    }
}
