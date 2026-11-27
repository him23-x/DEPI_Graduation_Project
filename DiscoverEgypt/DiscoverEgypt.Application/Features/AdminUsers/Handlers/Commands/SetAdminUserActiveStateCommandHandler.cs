using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.AdminUsers;
using DiscoverEgypt.Application.Features.AdminUsers.Requests.Commands;
using DiscoverEgypt.Application.Responses.AdminUsers;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminUsers.Handlers.Commands
{
    public sealed class SetAdminUserActiveStateCommandHandler
        : IRequestHandler<SetAdminUserActiveStateCommand, AdminUserStateChangeResponse>
    {
        private readonly IAdminUserManagementRepository _repository;
        private readonly IValidator<SetAdminUserActiveStateCommand> _validator;

        public SetAdminUserActiveStateCommandHandler(
            IAdminUserManagementRepository repository,
            IValidator<SetAdminUserActiveStateCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<AdminUserStateChangeResponse> Handle(
            SetAdminUserActiveStateCommand request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var updated = await _repository.SetActiveStateAsync(request.UserId, request.Form.IsActive, cancellationToken);
            return new AdminUserStateChangeResponse(
                updated ? AdminUserStateChangeOutcome.Success : AdminUserStateChangeOutcome.UserUnavailable,
                request.UserId,
                request.Form.IsActive);
        }
    }
}
