using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries;

public sealed class GetHostExperienceCreateOptionsQueryHandler(IHostExperienceRepository repository)
    : IRequestHandler<GetHostExperienceCreateOptionsQuery, HostExperienceCreateOptionsDto>
{
    public Task<HostExperienceCreateOptionsDto> Handle(GetHostExperienceCreateOptionsQuery request, CancellationToken cancellationToken)
        => repository.GetCreateOptionsAsync(cancellationToken);
}
