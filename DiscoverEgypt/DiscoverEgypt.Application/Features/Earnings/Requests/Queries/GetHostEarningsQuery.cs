using DiscoverEgypt.Application.DTOs.Earnings;
using DiscoverEgypt.Domain.Enums.Payments;
using MediatR;

namespace DiscoverEgypt.Application.Features.Earnings.Requests.Queries;

public sealed record GetHostEarningsQuery(
    string UserId,
    PayoutStatus? Status = null,
    int Page = 1,
    int PageSize = 10) : IRequest<HostEarningsDto>;
