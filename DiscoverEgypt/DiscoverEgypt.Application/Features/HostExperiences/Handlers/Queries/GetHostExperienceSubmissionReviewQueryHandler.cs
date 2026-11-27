using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Validators.DTOs;
using MediatR;
using AutoMapper;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries
{
    public sealed class GetHostExperienceSubmissionReviewQueryHandler
        : IRequestHandler<GetHostExperienceSubmissionReviewQuery, ExperienceSubmissionReviewDto?>
    {
        private readonly IHostExperienceSubmissionRepository _repository;
        private readonly HostExperienceSubmissionReviewDtoValidator _reviewValidator;
        private readonly IMapper _mapper;

        public GetHostExperienceSubmissionReviewQueryHandler(
            IHostExperienceSubmissionRepository repository,
            HostExperienceSubmissionReviewDtoValidator reviewValidator,
            IMapper mapper)
        {
            _repository = repository;
            _reviewValidator = reviewValidator;
            _mapper = mapper;
        }

        public async Task<ExperienceSubmissionReviewDto?> Handle(GetHostExperienceSubmissionReviewQuery request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var experience = await _repository.GetReviewAsync(request.UserId, request.ExperienceId, cancellationToken);
            var review = experience is null ? null : _mapper.Map<ExperienceSubmissionReviewDto>(experience);
            return review is null ? null : review with { Issues = _reviewValidator.GetIssues(review) };
        }
    }
}

