using DiscoverEgypt.Application.Contracts.Identity;
using DiscoverEgypt.Application.Enums.Accounts;
using DiscoverEgypt.Application.Responses.Accounts;
using Microsoft.AspNetCore.Identity;

namespace DiscoverEgypt.Persistence.Identity
{
    public sealed class IdentityAccountService : IAccountIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityAccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AccountAuthenticationResponse> LoginAsync(
            string email,
            string password,
            bool rememberMe,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null || !user.IsActive)
                return InvalidCredentials();

            var signIn = await _signInManager.PasswordSignInAsync(
                user, password, rememberMe, lockoutOnFailure: true);
            if (!signIn.Succeeded)
                return InvalidCredentials();

            var isHost = await _userManager.IsInRoleAsync(user, IdentityRoles.Host);
            var isTourist = await _userManager.IsInRoleAsync(user, IdentityRoles.Tourist);
            return new AccountAuthenticationResponse(true, isHost, [], isTourist);
        }

        public async Task<AccountAuthenticationResponse> RegisterAsync(
            string email,
            string fullName,
            string password,
            UserRole role,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role is not (UserRole.Tourist or UserRole.Host or UserRole.Guide))
                return new AccountAuthenticationResponse(false, false, ["Choose Tourist, Host, or Guide."]);

            var normalizedEmail = email.Trim();
            var user = new ApplicationUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail,
                FullName = fullName.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                return new AccountAuthenticationResponse(
                    false, false, createResult.Errors.Select(error => error.Description).ToArray());

            var roleResult = await _userManager.AddToRoleAsync(user, role.ToString());
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return new AccountAuthenticationResponse(
                    false, false, roleResult.Errors.Select(error => error.Description).ToArray());
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return new AccountAuthenticationResponse(true, false, []);
        }

        private static AccountAuthenticationResponse InvalidCredentials() =>
            new(false, false, ["The email or password is incorrect."]);
    }
}
