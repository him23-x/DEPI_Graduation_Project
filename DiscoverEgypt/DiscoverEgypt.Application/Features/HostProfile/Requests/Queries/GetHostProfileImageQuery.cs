using DiscoverEgypt.Application.Contracts.Storage;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostProfile.Requests.Queries
{
    public sealed record GetHostProfileImageQuery(string UserId) : IRequest<ExperienceMediaDownload?>;
}
