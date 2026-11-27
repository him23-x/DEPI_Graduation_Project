using DiscoverEgypt.Application.Features.AdminExperiences.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.AdminExperiences.Validators
{
    public sealed class GetAdminExperienceDetailsQueryValidator : AbstractValidator<GetAdminExperienceDetailsQuery>
    {
        public GetAdminExperienceDetailsQueryValidator() =>
            RuleFor(query => query.ExperienceId).GreaterThan(0);
    }
}
