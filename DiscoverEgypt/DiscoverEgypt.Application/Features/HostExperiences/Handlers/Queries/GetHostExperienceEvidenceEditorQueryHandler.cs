using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.ViewModels.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;
using AutoMapper;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries
{
    public sealed class GetHostExperienceEvidenceEditorQueryHandler
        : IRequestHandler<GetHostExperienceEvidenceEditorQuery, ExperienceEvidenceEditorViewModel?>
    {
        private readonly IHostExperienceEvidenceRepository _repository;
        private readonly IMapper _mapper;

        public GetHostExperienceEvidenceEditorQueryHandler(IHostExperienceEvidenceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ExperienceEvidenceEditorViewModel?> Handle(GetHostExperienceEvidenceEditorQuery request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var experience = await _repository.GetDraftEditorAsync(request.UserId, request.ExperienceId, cancellationToken);
            if (experience is null) return null;

            var model = _mapper.Map<ExperienceEvidenceEditorViewModel>(experience);
            var textKind = HostExperienceSubmissionPolicy.GetTextKind(model.ExperienceType);
            return model with
            {
                TextContent = textKind is null ? null : experience.Evidence.FirstOrDefault(item => item.EvidenceKind == textKind)?.TextContent,
                Files = _mapper.Map<IReadOnlyList<ExperienceEvidenceFileViewModel>>(experience.Evidence
                    .Where(item => item.TextContent is null)
                    .OrderBy(item => item.EvidenceKind)
                    .ThenBy(item => item.UploadedAt))
            };
        }
    }
}

