using FluentValidation;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands.Validation;

public sealed class UpdateHostExperienceDraftCommandValidator : AbstractValidator<UpdateHostExperienceDraftCommand>
{
    public UpdateHostExperienceDraftCommandValidator(IValidator<CreateHostExperienceDraftRequest> draftValidator)
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ExperienceId).GreaterThan(0);
        RuleFor(x => x.Draft).NotNull().SetValidator(draftValidator);
    }
}
