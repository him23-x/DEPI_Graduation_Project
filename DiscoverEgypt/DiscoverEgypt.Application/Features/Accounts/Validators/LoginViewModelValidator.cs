using DiscoverEgypt.Application.ViewModels.Account;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Accounts.Validators
{
    public sealed class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(model => model.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Enter a valid email address.");

            RuleFor(model => model.Password)
                .NotEmpty()
                .WithMessage("Enter your password.");
        }
    }
}
