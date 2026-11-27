using DiscoverEgypt.Application.Responses.Accounts;
using MediatR;

namespace DiscoverEgypt.Application.Features.Accounts.Requests
{
    public sealed record LoginAccountCommand(string Email, string Password, bool RememberMe) : IRequest<AccountAuthenticationResponse>;
}
