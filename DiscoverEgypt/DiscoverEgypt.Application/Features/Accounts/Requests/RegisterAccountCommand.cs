using DiscoverEgypt.Application.ViewModels.Account;
using MediatR;

namespace DiscoverEgypt.Application.Features.Accounts.Requests
{
    public sealed record RegisterAccountCommand(RegisterViewModel Model) : IRequest<AccountAuthenticationResult>;
}
