using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;
using AutoMapper;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries
{
    public sealed class GetHostExperienceCreateOptionsQueryHandler
        : IRequestHandler<GetHostExperienceCreateOptionsQuery, ExperienceCreateOptionsDto>
    {
        private readonly IHostExperienceRepository _repository;
        private readonly IMapper _mapper;

        public GetHostExperienceCreateOptionsQueryHandler(IHostExperienceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ExperienceCreateOptionsDto> Handle(GetHostExperienceCreateOptionsQuery request, CancellationToken cancellationToken)
        {
            var types = await _repository.GetExperienceTypesAsync(cancellationToken);
            var areas = await _repository.GetCulturalAreasAsync(cancellationToken);
            var interests = await _repository.GetInterestsAsync(cancellationToken);
            var languages = await _repository.GetLanguagesAsync(cancellationToken);

            return new ExperienceCreateOptionsDto(
                _mapper.Map<IReadOnlyList<ExperienceLookupDto>>(types),
                _mapper.Map<IReadOnlyList<ExperienceLookupDto>>(areas),
                _mapper.Map<IReadOnlyList<ExperienceLookupDto>>(interests),
                _mapper.Map<IReadOnlyList<ExperienceLookupDto>>(languages));
        }
    }
}

