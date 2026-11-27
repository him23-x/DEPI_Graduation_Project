using DiscoverEgypt.Application.Features.ExperienceDetails.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.ExperienceDetails.Validators
{
    public sealed class GetPublishedExperienceDetailsQueryValidator : AbstractValidator<GetPublishedExperienceDetailsQuery>
    {
        public GetPublishedExperienceDetailsQueryValidator() => RuleFor(query => query.ExperienceId).GreaterThan(0);
    }

    public sealed class GetPublishedExperienceDetailsPhotoKeyQueryValidator : AbstractValidator<GetPublishedExperienceDetailsPhotoKeyQuery>
    {
        public GetPublishedExperienceDetailsPhotoKeyQueryValidator()
        {
            RuleFor(query => query.ExperienceId).GreaterThan(0);
            RuleFor(query => query.MediaId).GreaterThan(0);
        }
    }
}
