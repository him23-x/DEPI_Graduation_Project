using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries;

public sealed class GetHostBookingsQueryHandler(IHostBookingRepository repository)
    : IRequestHandler<GetHostBookingsQuery, IReadOnlyList<HostBookingListItemDto>>
{
    public Task<IReadOnlyList<HostBookingListItemDto>> Handle(GetHostBookingsQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.GetForHostAsync(request.UserId, request.Tab, cancellationToken);
    }
}
