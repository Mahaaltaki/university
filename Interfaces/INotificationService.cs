using System;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Notification;// For NotificationDto
using System.Collections.Generic;
using System.Threading.Tasks;
namespace kalamon_University.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Creates and stores a notification for a specific user.
        /// Can also trigger other notification mechanisms (e.g., email, SignalR).
        /// </summary>
        Task<ServiceResult<NotificationDto>> SendNotificationAsync(CreateNotificationDto notificationDetails);

        /// <summary>
        /// Sends a notification to multiple target users.
        /// </summary>
        Task<ServiceResult> SendBulkNotificationAsync(IEnumerable<Guid> targetUserIds, string message, string? relatedEntityType = null, int? relatedEntityId = null);


        /// <summary>
        /// Retrieves notifications for a specific user.
        /// </summary>
        Task<ServiceResult<IEnumerable<NotificationDto>>> GetNotificationsForUserAsync(Guid targetUserId, bool onlyUnread = false, int page = 1, int pageSize = 10);

        /// <summary>
        /// Marks a specific notification as read.
        /// </summary>
        Task<ServiceResult> MarkAsReadAsync(Guid targetUserId, int notificationId);

        /// <summary>
        /// Marks all unread notifications for a user as read.
        /// </summary>
        Task<ServiceResult> MarkAllAsReadAsync(Guid targetUserId);

        /// <summary>
        /// Gets the count of unread notifications for a user.
        /// </summary>
        Task<ServiceResult<int>> GetUnreadNotificationCountAsync(Guid targetUserId);
    }
}