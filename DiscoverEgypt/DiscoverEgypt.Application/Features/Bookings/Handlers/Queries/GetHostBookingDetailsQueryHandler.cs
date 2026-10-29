using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries;

public sealed class GetHostBookingDetailsQueryHandler(IHostBookingRepository repository)
    : IRequestHandler<GetHostBookingDetailsQuery, HostBookingDetailDto?>
{
    public Task<HostBookingDetailDto?> Handle(GetHostBookingDetailsQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.GetDetailsAsync(request.UserId, request.BookingId, cancellationToken);
    }
}
