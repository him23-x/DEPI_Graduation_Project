using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries;

public sealed class GetHostExperienceListRequestHandler(IHostExperienceRepository repository)
    : IRequestHandler<GetHostExperienceListQuery, IReadOnlyList<HostExperienceListItemDto>>
{
    public Task<IReadOnlyList<HostExperienceListItemDto>> Handle(
        GetHostExperienceListQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.GetForHostAsync(request.UserId, request.Status, cancellationToken);
    }
}
