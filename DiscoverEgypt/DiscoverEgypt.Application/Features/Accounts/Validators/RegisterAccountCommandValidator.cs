using DiscoverEgypt.Application.Enums.Accounts;
using DiscoverEgypt.Application.Features.Accounts.Requests;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Accounts.Validators
{
    public sealed class RegisterAccountCommandValidator : AbstractValidator<RegisterAccountCommand>
    {
        public RegisterAccountCommandValidator()
        {
            RuleFor(command => command.FullName)
                .NotEmpty()
                .Length(2, 150);

            RuleFor(command => command.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(command => command.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(command => command.ConfirmPassword)
                .Equal(command => command.Password)
                .WithMessage("The password confirmation does not match.");

            RuleFor(command => command.Role)
                .Must(role => role.HasValue && Enum.IsDefined(role.Value)
                    && role.Value is UserRole.Tourist or UserRole.Host or UserRole.Guide)
                .WithMessage("Choose Tourist, Host, or Guide.");

            RuleFor(command => command.AcceptTerms)
                .Equal(true)
                .WithMessage("Accept the terms to create an account.");
        }
    }
}
