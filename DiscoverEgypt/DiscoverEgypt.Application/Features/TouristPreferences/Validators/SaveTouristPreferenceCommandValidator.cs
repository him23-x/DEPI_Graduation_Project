using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.TouristPreferences.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.TouristPreferences.Validators
{
    public sealed class SaveTouristPreferenceCommandValidator : AbstractValidator<SaveTouristPreferenceCommand>
    {
        public SaveTouristPreferenceCommandValidator(ITouristPreferenceRepository repository)
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.Form).NotNull();

            When(command => command.Form is not null, () =>
            {
                RuleFor(command => command.Form.Budget).NotNull().IsInEnum();
                RuleFor(command => command.Form.Duration).NotNull().IsInEnum();
                RuleFor(command => command.Form.GuidePreference).NotNull().IsInEnum();
                RuleFor(command => command.Form.InterestIds).Must(HaveNoDuplicates).WithMessage("Remove duplicate interests.");
                RuleFor(command => command.Form.ExperienceTypeIds).Must(HaveNoDuplicates).WithMessage("Remove duplicate experience types.");
                RuleFor(command => command.Form.CulturalAreaIds).Must(HaveNoDuplicates).WithMessage("Remove duplicate cultural areas.");
                RuleFor(command => command.Form.LanguageIds).Must(HaveNoDuplicates).WithMessage("Remove duplicate languages.");
                RuleFor(command => command.Form).MustAsync((form, cancellationToken) => repository.AreLookupSelectionsValidAsync(
                    form.InterestIds, form.ExperienceTypeIds, form.CulturalAreaIds, form.LanguageIds, cancellationToken))
                    .WithMessage("One or more selected options are no longer available. Refresh the page and try again.");
            });
        }

        private static bool HaveNoDuplicates(ICollection<int>? ids) => ids is not null && ids.Count == ids.Distinct().Count();
    }
}
