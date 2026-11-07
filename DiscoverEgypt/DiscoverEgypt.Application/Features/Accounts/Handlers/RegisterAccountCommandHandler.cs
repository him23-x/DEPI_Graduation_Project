using DiscoverEgypt.Application.Features.Accounts.Requests;
using DiscoverEgypt.Application.ViewModels.Account;
using DiscoverEgypt.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DiscoverEgypt.Application.Features.Accounts.Handlers
{
    public sealed class RegisterAccountCommandHandler : IRequestHandler<RegisterAccountCommand, AccountAuthenticationResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IValidator<RegisterViewModel> _validator;

        public RegisterAccountCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IValidator<RegisterViewModel> validator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _validator = validator;
        }

        public async Task<AccountAuthenticationResult> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.Model, cancellationToken);
            var user = new ApplicationUser
            {
                UserName = request.Model.Email.Trim(),
                Email = request.Model.Email.Trim(),
                FullName = request.Model.FullName.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, request.Model.Password);
            if (!createResult.Succeeded)
                return new AccountAuthenticationResult(false, false, createResult.Errors.Select(error => error.Description).ToArray());

            var roleResult = await _userManager.AddToRoleAsync(user, request.Model.Role!.Value.ToString());
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return new AccountAuthenticationResult(false, false, roleResult.Errors.Select(error => error.Description).ToArray());
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return new AccountAuthenticationResult(true, false, []);
        }
    }
}
