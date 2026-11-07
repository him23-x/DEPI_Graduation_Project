using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.ViewModels.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Validators.ViewModels;
using MediatR;
using AutoMapper;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries
{
    public sealed class GetHostExperienceSubmissionReviewQueryHandler
        : IRequestHandler<GetHostExperienceSubmissionReviewQuery, ExperienceSubmissionReviewViewModel?>
    {
        private readonly IHostExperienceSubmissionRepository _repository;
        private readonly HostExperienceSubmissionReviewValidator _reviewValidator;
        private readonly IMapper _mapper;

        public GetHostExperienceSubmissionReviewQueryHandler(
            IHostExperienceSubmissionRepository repository,
            HostExperienceSubmissionReviewValidator reviewValidator,
            IMapper mapper)
        {
            _repository = repository;
            _reviewValidator = reviewValidator;
            _mapper = mapper;
        }

        public async Task<ExperienceSubmissionReviewViewModel?> Handle(GetHostExperienceSubmissionReviewQuery request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var experience = await _repository.GetReviewAsync(request.UserId, request.ExperienceId, cancellationToken);
            var review = experience is null ? null : _mapper.Map<ExperienceSubmissionReviewViewModel>(experience);
            return review is null ? null : review with { Issues = _reviewValidator.GetIssues(review) };
        }
    }
}

