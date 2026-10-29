using FluentValidation;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

namespace DiscoverEgypt.Application.Features.HostExperiences.Validators.Commands;

public sealed class UpdateHostExperienceDraftCommandValidator : AbstractValidator<UpdateHostExperienceDraftCommand>
{
    public UpdateHostExperienceDraftCommandValidator(IValidator<CreateHostExperienceDraftDto> draftValidator)
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ExperienceId).GreaterThan(0);
        RuleFor(x => x.Draft).NotNull().SetValidator(draftValidator);
    }
}
