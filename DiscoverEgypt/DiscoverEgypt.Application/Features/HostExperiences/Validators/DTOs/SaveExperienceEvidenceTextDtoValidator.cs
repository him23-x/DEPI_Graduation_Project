using DiscoverEgypt.Application.DTOs.HostExperiences;
using FluentValidation;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Validators.DTOs;

public sealed class SaveExperienceEvidenceTextDtoValidator : AbstractValidator<SaveExperienceEvidenceTextDto>
{
    public SaveExperienceEvidenceTextDtoValidator()
    {
        RuleFor(x => x.ExperienceId).GreaterThan(0);
        RuleFor(x => x.Kind).Must(kind => Enum.IsDefined(kind)).WithMessage("Choose a valid evidence type.");
        RuleFor(x => x.TextContent).MaximumLength(5000);
        RuleFor(x => x.TextContent).NotEmpty().When(x => x.Kind is EvidenceKind.MenuAndAllergenInfo or EvidenceKind.RouteDetails)
            .WithMessage("This evidence text is required.");
    }
}
