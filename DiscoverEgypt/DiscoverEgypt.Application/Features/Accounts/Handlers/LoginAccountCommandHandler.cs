using DiscoverEgypt.Application.Features.Accounts.Requests;
using DiscoverEgypt.Application.Contracts.Identity;
using DiscoverEgypt.Application.Responses.Accounts;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.Accounts.Handlers
{
    public sealed class LoginAccountCommandHandler : IRequestHandler<LoginAccountCommand, AccountAuthenticationResponse>
    {
        private readonly IAccountIdentityService _accountIdentityService;
        private readonly IValidator<LoginAccountCommand> _validator;

        public LoginAccountCommandHandler(
            IAccountIdentityService accountIdentityService,
            IValidator<LoginAccountCommand> validator)
        {
            _accountIdentityService = accountIdentityService;
            _validator = validator;
        }

        public async Task<AccountAuthenticationResponse> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            return await _accountIdentityService.LoginAsync(
                request.Email,
                request.Password,
                request.RememberMe,
                cancellationToken);
        }
    }
}
