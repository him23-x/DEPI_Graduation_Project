using DiscoverEgypt.Application.DTOs.Disputes;
using DiscoverEgypt.Application.Responses.Disputes;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Requests.Commands
{
    public sealed record OpenTouristDisputeCommand(string UserId, OpenTouristDisputeDto Form)
        : IRequest<TouristDisputeOpenResponse>;
}
