using kalamon_University.Interfaces;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Notification;
using kalamon_University.Data;
using kalamon_University.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace kalamon_University.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<NotificationDto>> SendNotificationAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                RelatedEntityType = dto.RelatedEntityType,
                RelatedEntityId = dto.RelatedEntityId,
                RelatedEntityName = dto.RelatedEntityName
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return ServiceResult<NotificationDto>.Succeeded(new NotificationDto(notification));
        }

        public async Task<ServiceResult> SendBulkNotificationAsync(IEnumerable<Guid> targetUserIds, string message, string? relatedEntityType = null, int? relatedEntityId = null)
        {
            var notifications = targetUserIds.Select(userId => new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId
            });

            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();

            return ServiceResult.Succeeded();
        }

        public async Task<ServiceResult<IEnumerable<NotificationDto>>> GetNotificationsForUserAsync(Guid userId, bool onlyUnread = false, int page = 1, int pageSize = 10)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId);

            if (onlyUnread)
                query = query.Where(n => !n.IsRead);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ServiceResult<IEnumerable<NotificationDto>>.Succeeded(
                notifications.Select(n => new NotificationDto(n))
            );
        }

        public async Task<ServiceResult> MarkAsReadAsync(Guid userId, int notificationId)
        {
            var notif = await _context.Notifications.FirstOrDefaultAsync(n => n.UserId == userId && n.Id == notificationId);
            if (notif is null)
                return ServiceResult.Failed("Notification not found.");

            notif.IsRead = true;
            await _context.SaveChangesAsync();
            return ServiceResult.Succeeded();
        }

        public async Task<ServiceResult> MarkAllAsReadAsync(Guid userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            unread.ForEach(n => n.IsRead = true);
            await _context.SaveChangesAsync();

            return ServiceResult.Succeeded();
        }

        public async Task<ServiceResult<int>> GetUnreadNotificationCountAsync(Guid userId)
        {
            var count = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return ServiceResult<int>.Succeeded(count);
        }
    }

}
