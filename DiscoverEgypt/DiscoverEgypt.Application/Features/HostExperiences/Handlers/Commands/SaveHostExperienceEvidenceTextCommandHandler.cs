using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;

using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;

public sealed class SaveHostExperienceEvidenceTextCommandHandler(IHostExperienceEvidenceRepository repository)
    : IRequestHandler<SaveHostExperienceEvidenceTextCommand, ExperienceEvidenceMutationOutcome>
{
    public Task<ExperienceEvidenceMutationOutcome> Handle(SaveHostExperienceEvidenceTextCommand request, CancellationToken cancellationToken)
    {
        if (request.Text is null) return Task.FromResult(ExperienceEvidenceMutationOutcome.TextRequired);
        if (request.Text.Length > 5000) return Task.FromResult(ExperienceEvidenceMutationOutcome.TextTooLong);
        return repository.SaveTextAsync(request.UserId, request.ExperienceId, request.Kind, request.Text, cancellationToken);
    }
}
