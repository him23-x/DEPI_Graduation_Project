using FluentValidation;
using DiscoverEgypt.Application.ViewModels.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;

namespace DiscoverEgypt.Application.Features.HostExperiences.Validators.Commands
{
    public sealed class CreateHostExperienceDraftCommandValidator : AbstractValidator<CreateHostExperienceDraftCommand>
    {
        public CreateHostExperienceDraftCommandValidator(IValidator<HostExperienceDraftViewModel> draftValidator)
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Draft).NotNull().SetValidator(draftValidator);
        }
    }
}

