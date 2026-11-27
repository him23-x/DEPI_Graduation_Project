using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Responses.AdminUsers;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Admin.Users
{
    public sealed class AdminUserManagementRepository : IAdminUserManagementRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public AdminUserManagementRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public async Task<IReadOnlyList<AdminUserListItemResponse>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var users = await _dbContext.Users.AsNoTracking()
                .OrderBy(user => user.FullName)
                .ThenBy(user => user.Email)
                .Select(user => new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.IsActive,
                    user.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var userRoles = await (
                from userRole in _dbContext.UserRoles.AsNoTracking()
                join role in _dbContext.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                select new { userRole.UserId, RoleName = role.Name ?? string.Empty })
                .ToListAsync(cancellationToken);

            return users.Select(user => new AdminUserListItemResponse(
                    user.Id,
                    user.FullName,
                    user.Email ?? string.Empty,
                    userRoles.Where(userRole => userRole.UserId == user.Id)
                        .Select(userRole => userRole.RoleName)
                        .OrderBy(role => role)
                        .ToArray(),
                    user.IsActive,
                    DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc)))
                .ToArray();
        }

        public async Task<bool> SetActiveStateAsync(
            string userId,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);
            if (user is null) return false;

            if (user.IsActive != isActive)
            {
                user.IsActive = isActive;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return true;
        }
    }
}
