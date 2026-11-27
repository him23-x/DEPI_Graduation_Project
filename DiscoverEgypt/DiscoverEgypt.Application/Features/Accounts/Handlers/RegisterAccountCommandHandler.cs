using DiscoverEgypt.Application.Features.Accounts.Requests;
using DiscoverEgypt.Application.Contracts.Identity;
using DiscoverEgypt.Application.Responses.Accounts;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.Accounts.Handlers
{
    public sealed class RegisterAccountCommandHandler : IRequestHandler<RegisterAccountCommand, AccountAuthenticationResponse>
    {
        private readonly IAccountIdentityService _accountIdentityService;
        private readonly IValidator<RegisterAccountCommand> _validator;

        public RegisterAccountCommandHandler(
            IAccountIdentityService accountIdentityService,
            IValidator<RegisterAccountCommand> validator)
        {
            _accountIdentityService = accountIdentityService;
            _validator = validator;
        }

        public async Task<AccountAuthenticationResponse> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            return await _accountIdentityService.RegisterAsync(
                request.Email,
                request.FullName,
                request.Password,
                request.Role!.Value,
                cancellationToken);
        }
    }
}
