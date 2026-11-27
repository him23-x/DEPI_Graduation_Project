using DiscoverEgypt.Application.Features.AdminUsers.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.AdminUsers.Validators
{
    public sealed class SetAdminUserActiveStateCommandValidator : AbstractValidator<SetAdminUserActiveStateCommand>
    {
        public SetAdminUserActiveStateCommandValidator()
        {
            RuleFor(command => command.AdminUserId).NotEmpty();
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.Form).NotNull();
        }
    }
}
