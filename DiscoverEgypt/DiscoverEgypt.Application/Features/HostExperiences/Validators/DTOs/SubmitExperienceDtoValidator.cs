using DiscoverEgypt.Application.DTOs.HostExperiences;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.HostExperiences.Validators.DTOs;

public sealed class SubmitExperienceDtoValidator : AbstractValidator<SubmitExperienceDto>
{
    public SubmitExperienceDtoValidator()
    {
        RuleFor(x => x.ExperienceId).GreaterThan(0);
        RuleFor(x => x.Accepted).Equal(true).WithMessage("Accept the declaration before submitting.");
        RuleFor(x => x.VenueUse).Must(value => !value.HasValue || Enum.IsDefined(value.Value))
            .WithMessage("Choose a valid venue type.");
    }
}
