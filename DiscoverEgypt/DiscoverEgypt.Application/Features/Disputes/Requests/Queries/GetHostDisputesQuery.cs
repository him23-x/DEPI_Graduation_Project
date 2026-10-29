using DiscoverEgypt.Application.DTOs.Disputes;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Requests.Queries;

public sealed record GetHostDisputesQuery(string UserId, int Page = 1, int PageSize = 10)
    : IRequest<HostDisputesPageDto>;
