using DiscoverEgypt.Application.DTOs.AdminUsers;
using DiscoverEgypt.Application.Responses.AdminUsers;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminUsers.Requests.Commands
{
    public sealed record SetAdminUserActiveStateCommand(
        string AdminUserId,
        string UserId,
        AdminUserStateDto Form) : IRequest<AdminUserStateChangeResponse>;
}
