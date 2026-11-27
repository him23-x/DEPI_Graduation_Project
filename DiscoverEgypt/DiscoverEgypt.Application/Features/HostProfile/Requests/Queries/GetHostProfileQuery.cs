using DiscoverEgypt.Application.DTOs.HostProfile;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostProfile.Requests.Queries
{
    public sealed record GetHostProfileQuery(string UserId) : IRequest<HostProfileDto?>;
}
