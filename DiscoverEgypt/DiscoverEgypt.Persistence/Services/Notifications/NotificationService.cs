using DiscoverEgypt.Application.Contracts.Notifications;
using DiscoverEgypt.Application.ViewModels.Notifications;
using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using DiscoverEgypt.Application.Enums.Notifications;

using DiscoverEgypt.Domain.Enums.Notifications;

namespace DiscoverEgypt.Persistence.Services.Notifications
{
    public sealed class NotificationService : INotificationService
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public NotificationService(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(
            string userId,
            NotificationType type,
            string message,
            int? bookingId = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);
            if (message.Length > 2000)
                throw new ArgumentOutOfRangeException(nameof(message), "Notification messages cannot exceed 2,000 characters.");

            _dbContext.Notifications.Add(new Notification
            {
                UserId = userId,
                BookingId = bookingId,
                Type = type,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<HostNotificationsViewModel> GetForUserAsync(
            string userId,
            NotificationReadFilter filter,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var userNotifications = _dbContext.Notifications.AsNoTracking()
                .Where(notification => notification.UserId == userId);
            var filteredNotifications = filter switch
            {
                NotificationReadFilter.Unread => userNotifications.Where(notification => !notification.IsRead),
                NotificationReadFilter.Read => userNotifications.Where(notification => notification.IsRead),
                _ => userNotifications
            };

            var totalCount = await filteredNotifications.CountAsync(cancellationToken);
            page = Math.Min(page, Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize)));
            var notifications = await filteredNotifications
                .OrderByDescending(notification => notification.CreatedAt)
                .ThenByDescending(notification => notification.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(notification => new HostNotificationViewModel(
                    notification.Id,
                    notification.Type,
                    notification.Message,
                    notification.IsRead,
                    notification.CreatedAt,
                    notification.BookingId))
                .ToListAsync(cancellationToken);
            var unreadCount = await userNotifications.CountAsync(notification => !notification.IsRead, cancellationToken);

            return new HostNotificationsViewModel(notifications, filter, page, pageSize, totalCount, unreadCount);
        }

        public Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default) =>
            _dbContext.Notifications.CountAsync(
                notification => notification.UserId == userId && !notification.IsRead,
                cancellationToken);

        public async Task<bool> MarkReadAsync(
            int notificationId,
            string userId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _dbContext.Notifications.SingleOrDefaultAsync(
                item => item.Id == notificationId && item.UserId == userId,
                cancellationToken);
            if (notification is null) return false;
            if (notification.IsRead) return true;

            notification.IsRead = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public Task<int> MarkAllReadAsync(string userId, CancellationToken cancellationToken = default) =>
            _dbContext.Notifications
                .Where(notification => notification.UserId == userId && !notification.IsRead)
                .ExecuteUpdateAsync(update => update.SetProperty(notification => notification.IsRead, true), cancellationToken);
    }
}

