using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.HostExperiences.Validators.Commands
{
    public sealed class ReturnPublishedExperienceToReviewCommandValidator : AbstractValidator<ReturnPublishedExperienceToReviewCommand>
    {
        public ReturnPublishedExperienceToReviewCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.ExperienceId).GreaterThan(0);
        }
    }
}
