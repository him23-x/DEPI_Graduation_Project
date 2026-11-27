using DiscoverEgypt.Application.Responses.AdminUsers;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IAdminUserManagementRepository
    {
        Task<IReadOnlyList<AdminUserListItemResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> SetActiveStateAsync(string userId, bool isActive, CancellationToken cancellationToken = default);
    }
}
