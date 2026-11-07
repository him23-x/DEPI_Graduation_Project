using DiscoverEgypt.Application.Features.Accounts.Requests;
using DiscoverEgypt.Application.Enums.Accounts;
using DiscoverEgypt.Application.ViewModels.Account;
using DiscoverEgypt.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DiscoverEgypt.Application.Features.Accounts.Handlers
{
    public sealed class LoginAccountCommandHandler : IRequestHandler<LoginAccountCommand, AccountAuthenticationResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IValidator<LoginViewModel> _validator;

        public LoginAccountCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IValidator<LoginViewModel> validator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _validator = validator;
        }

        public async Task<AccountAuthenticationResult> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.Model, cancellationToken);
            var user = await _userManager.FindByEmailAsync(request.Model.Email);
            if (user is null || !user.IsActive)
                return new AccountAuthenticationResult(false, false, ["The email or password is incorrect."]);

            var result = await _signInManager.PasswordSignInAsync(user, request.Model.Password, request.Model.RememberMe, lockoutOnFailure: true);
            if (!result.Succeeded)
                return new AccountAuthenticationResult(false, false, ["The email or password is incorrect."]);

            return new AccountAuthenticationResult(true, await _userManager.IsInRoleAsync(user, UserRole.Host.ToString()), []);
        }
    }
}
