using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.ViewModels.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;
using AutoMapper;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries
{
    public sealed class GetEditableHostExperienceDraftQueryHandler
        : IRequestHandler<GetEditableHostExperienceDraftQuery, HostExperienceDraftEditorViewModel?>
    {
        private readonly IHostExperienceRepository _repository;
        private readonly IMapper _mapper;

        public GetEditableHostExperienceDraftQueryHandler(IHostExperienceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<HostExperienceDraftEditorViewModel?> Handle(GetEditableHostExperienceDraftQuery request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var experience = await _repository.GetEditableDraftAsync(request.UserId, request.ExperienceId, cancellationToken);
            return experience is null ? null : _mapper.Map<HostExperienceDraftEditorViewModel>(experience);
        }
    }
}

