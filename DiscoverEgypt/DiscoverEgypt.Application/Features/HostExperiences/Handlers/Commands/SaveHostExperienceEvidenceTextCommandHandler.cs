using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Domain.Entities;
using MediatR;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands
{
    public sealed class SaveHostExperienceEvidenceTextCommandHandler : IRequestHandler<SaveHostExperienceEvidenceTextCommand, ExperienceEvidenceMutationOutcome>
    {
        private readonly IHostExperienceEvidenceRepository _repository;
        private readonly IValidator<SaveHostExperienceEvidenceTextCommand> _validator;

        public SaveHostExperienceEvidenceTextCommandHandler(
            IHostExperienceEvidenceRepository repository,
            IValidator<SaveHostExperienceEvidenceTextCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<ExperienceEvidenceMutationOutcome> Handle(
            SaveHostExperienceEvidenceTextCommand request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return ExperienceEvidenceMutationOutcome.TextRequired;
            }

            return await _repository.ExecuteMutationAsync(
                request.UserId,
                request.ExperienceId,
                (context, _) =>
                {
                    var experience = context.Experience;
                    if (!HostExperienceDraftPolicy.IsEditable(experience.Status))
                    {
                        return Task.FromResult(ExperienceEvidenceMutationOutcome.ExperienceUnavailable);
                    }

                    var experienceType = HostExperienceSubmissionPolicy.Classify(experience.ExperienceType.Name);
                    if (HostExperienceSubmissionPolicy.GetTextKind(experienceType) != request.Kind)
                    {
                        return Task.FromResult(ExperienceEvidenceMutationOutcome.InvalidEvidenceKind);
                    }

                    var normalizedText = request.Text.Trim();
                    var evidence = experience.Evidence.FirstOrDefault(item =>
                        item.EvidenceKind == request.Kind && item.DeletedAt == null && item.TextContent != null);
                    if (evidence is null)
                    {
                        experience.Evidence.Add(new ExperienceEvidence
                        {
                            EvidenceKind = request.Kind,
                            TextContent = normalizedText,
                            UploadedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        evidence.TextContent = normalizedText;
                        evidence.UploadedAt = DateTime.UtcNow;
                    }

                    return Task.FromResult(ExperienceEvidenceMutationOutcome.Success);
                },
                cancellationToken);
        }
    }
}
