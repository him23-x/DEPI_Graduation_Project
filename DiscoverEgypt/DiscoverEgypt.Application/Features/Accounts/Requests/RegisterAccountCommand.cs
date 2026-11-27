using DiscoverEgypt.Application.Enums.Accounts;
using DiscoverEgypt.Application.Responses.Accounts;
using MediatR;

namespace DiscoverEgypt.Application.Features.Accounts.Requests
{
    public sealed record RegisterAccountCommand(
        string FullName,
        string Email,
        string Password,
        string ConfirmPassword,
        UserRole? Role,
        bool AcceptTerms) : IRequest<AccountAuthenticationResponse>;
}
