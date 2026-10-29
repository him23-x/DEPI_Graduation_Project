using DiscoverEgypt.Application.DTOs.Bookings;
using MediatR;

using DiscoverEgypt.Application.Enums.Bookings;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Queries;

public sealed record GetHostBookingsQuery(string UserId, HostBookingTab Tab)
    : IRequest<IReadOnlyList<HostBookingListItemDto>>;
