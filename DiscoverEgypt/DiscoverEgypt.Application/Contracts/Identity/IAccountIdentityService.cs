using DiscoverEgypt.Application.Enums.Accounts;
using DiscoverEgypt.Application.Responses.Accounts;

namespace DiscoverEgypt.Application.Contracts.Identity
{
    public interface IAccountIdentityService
    {
        Task<AccountAuthenticationResponse> LoginAsync(
            string email,
            string password,
            bool rememberMe,
            CancellationToken cancellationToken = default);

        Task<AccountAuthenticationResponse> RegisterAsync(
            string email,
            string fullName,
            string password,
            UserRole role,
            CancellationToken cancellationToken = default);
    }
}
