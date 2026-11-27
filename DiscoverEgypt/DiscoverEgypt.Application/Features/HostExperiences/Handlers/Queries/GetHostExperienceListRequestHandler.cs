using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;
using AutoMapper;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries
{
    public sealed class GetHostExperienceListRequestHandler
        : IRequestHandler<GetHostExperienceListQuery, IReadOnlyList<ExperienceListItemDto>>
    {
        private readonly IHostExperienceRepository _repository;
        private readonly IMapper _mapper;

        public GetHostExperienceListRequestHandler(IHostExperienceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<ExperienceListItemDto>> Handle(
            GetHostExperienceListQuery request,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var experiences = await _repository.GetForHostAsync(request.UserId, request.Status, cancellationToken);
            return _mapper.Map<IReadOnlyList<ExperienceListItemDto>>(experiences);
        }
    }
}

