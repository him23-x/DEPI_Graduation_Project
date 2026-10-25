using FluentValidation;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

namespace DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands.Validation;

public sealed class CreateHostExperienceDraftCommandValidator : AbstractValidator<CreateHostExperienceDraftCommand>
{
    public CreateHostExperienceDraftCommandValidator(IValidator<CreateHostExperienceDraftRequest> draftValidator)
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Draft).NotNull().SetValidator(draftValidator);
    }
}
