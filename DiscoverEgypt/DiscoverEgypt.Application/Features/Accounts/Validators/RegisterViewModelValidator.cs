using DiscoverEgypt.Application.Enums.Accounts;
using DiscoverEgypt.Application.ViewModels.Account;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Accounts.Validators
{
    public sealed class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(model => model.FullName)
                .NotEmpty()
                .Length(2, 150);

            RuleFor(model => model.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(model => model.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(model => model.ConfirmPassword)
                .Equal(model => model.Password)
                .WithMessage("The password confirmation does not match.");

            RuleFor(model => model.Role)
                .Must(role => role.HasValue && Enum.IsDefined(role.Value)
                    && role.Value is UserRole.Tourist or UserRole.Host or UserRole.Guide)
                .WithMessage("Choose Tourist, Host, or Guide.");

            RuleFor(model => model.AcceptTerms)
                .Equal(true)
                .WithMessage("Accept the terms to create an account.");
        }
    }
}
