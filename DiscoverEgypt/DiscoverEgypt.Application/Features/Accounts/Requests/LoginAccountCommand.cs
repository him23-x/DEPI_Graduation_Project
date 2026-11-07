using DiscoverEgypt.Application.ViewModels.Account;
using MediatR;

namespace DiscoverEgypt.Application.Features.Accounts.Requests
{
    public sealed record LoginAccountCommand(LoginViewModel Model) : IRequest<AccountAuthenticationResult>;
}
