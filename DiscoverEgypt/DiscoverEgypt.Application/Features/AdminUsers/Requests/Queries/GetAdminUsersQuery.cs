using DiscoverEgypt.Application.Responses.AdminUsers;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminUsers.Requests.Queries
{
    public sealed record GetAdminUsersQuery : IRequest<IReadOnlyList<AdminUserListItemResponse>>;
}
