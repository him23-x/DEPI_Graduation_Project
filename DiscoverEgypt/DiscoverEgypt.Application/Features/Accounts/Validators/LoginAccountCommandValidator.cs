using DiscoverEgypt.Application.Features.Accounts.Requests;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Accounts.Validators
{
    public sealed class LoginAccountCommandValidator : AbstractValidator<LoginAccountCommand>
    {
        public LoginAccountCommandValidator()
        {
            RuleFor(command => command.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Enter a valid email address.");

            RuleFor(command => command.Password)
                .NotEmpty()
                .WithMessage("Enter your password.");
        }
    }
}
