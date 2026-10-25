using DiscoverEgypt.Application.DTOs.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Queries;

public sealed record GetHostBookingsQuery(string UserId, HostBookingTab Tab)
    : IRequest<IReadOnlyList<HostBookingListItem>>;
