using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.ViewModels.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;
using AutoMapper;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries
{
    public sealed class GetHostExperienceCreateOptionsQueryHandler
        : IRequestHandler<GetHostExperienceCreateOptionsQuery, ExperienceCreateOptionsViewModel>
    {
        private readonly IHostExperienceRepository _repository;
        private readonly IMapper _mapper;

        public GetHostExperienceCreateOptionsQueryHandler(IHostExperienceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ExperienceCreateOptionsViewModel> Handle(GetHostExperienceCreateOptionsQuery request, CancellationToken cancellationToken)
        {
            var types = await _repository.GetExperienceTypesAsync(cancellationToken);
            var areas = await _repository.GetCulturalAreasAsync(cancellationToken);
            var interests = await _repository.GetInterestsAsync(cancellationToken);
            var languages = await _repository.GetLanguagesAsync(cancellationToken);

            return new ExperienceCreateOptionsViewModel(
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(types),
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(areas),
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(interests),
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(languages));
        }
    }
}

