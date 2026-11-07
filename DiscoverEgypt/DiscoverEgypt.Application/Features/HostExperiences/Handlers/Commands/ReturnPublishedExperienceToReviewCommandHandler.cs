using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Domain.Enums.Experiences;
using MediatR;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands
{
    public sealed class ReturnPublishedExperienceToReviewCommandHandler : IRequestHandler<ReturnPublishedExperienceToReviewCommand, bool>
    {
        private readonly IExperienceStatusRepository _repository;
        private readonly IValidator<ReturnPublishedExperienceToReviewCommand> _validator;

        public ReturnPublishedExperienceToReviewCommandHandler(IExperienceStatusRepository repository, IValidator<ReturnPublishedExperienceToReviewCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<bool> Handle(
            ReturnPublishedExperienceToReviewCommand request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var currentStatus = await _repository.GetStatusAsync(request.UserId, request.ExperienceId, cancellationToken);
            if (currentStatus != ExperienceStatus.Published)
            {
                return false;
            }

            return await _repository.UpdateStatusAsync(
                request.UserId,
                request.ExperienceId,
                ExperienceStatus.Published,
                ExperienceStatus.PendingApproval,
                null,
                DateTime.UtcNow,
                cancellationToken);
        }
    }
}
