using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.AdminUsers.Requests.Queries;
using DiscoverEgypt.Application.Responses.AdminUsers;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminUsers.Handlers.Queries
{
    public sealed class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, IReadOnlyList<AdminUserListItemResponse>>
    {
        private readonly IAdminUserManagementRepository _repository;

        public GetAdminUsersQueryHandler(IAdminUserManagementRepository repository) => _repository = repository;

        public Task<IReadOnlyList<AdminUserListItemResponse>> Handle(
            GetAdminUsersQuery request,
            CancellationToken cancellationToken) => _repository.GetAllAsync(cancellationToken);
    }
}
