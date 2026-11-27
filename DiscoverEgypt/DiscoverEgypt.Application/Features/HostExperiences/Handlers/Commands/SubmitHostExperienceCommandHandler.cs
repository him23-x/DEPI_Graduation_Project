using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.HostExperiences;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using MediatR;
using AutoMapper;
using FluentValidation;

using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands
{
    public sealed class SubmitHostExperienceCommandHandler
        : IRequestHandler<SubmitHostExperienceCommand, ExperienceSubmissionOutcome>
    {
        private readonly IHostExperienceSubmissionRepository _repository;
        private readonly IExperienceReadinessService _readinessService;
        private readonly IMapper _mapper;
        private readonly IValidator<SubmitHostExperienceCommand> _validator;

        public SubmitHostExperienceCommandHandler(
            IHostExperienceSubmissionRepository repository,
            IExperienceReadinessService readinessService,
            IMapper mapper,
            IValidator<SubmitHostExperienceCommand> validator)
        {
            _repository = repository;
            _readinessService = readinessService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ExperienceSubmissionOutcome> Handle(SubmitHostExperienceCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);

            var experience = await _repository.GetReviewAsync(request.UserId, request.ExperienceId, cancellationToken);
            if (experience is null) return ExperienceSubmissionOutcome.ExperienceUnavailable;
            var review = _mapper.Map<ExperienceSubmissionReviewDto>(experience);
            var declaration = HostExperienceSubmissionPolicy.GetDeclaration(review.ExperienceType);
            if (declaration is null) return ExperienceSubmissionOutcome.UnsupportedType;
            if (_readinessService.GetIssues(review, request.VenueUse, request.EvidenceInHostName).Count > 0)
            {
                return ExperienceSubmissionOutcome.Incomplete;
            }

            return await _repository.SubmitAsync(
                request.UserId,
                request.ExperienceId,
                declaration.Type,
                declaration.Text,
                cancellationToken);
        }
    }
}

