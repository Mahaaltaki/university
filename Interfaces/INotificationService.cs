using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Notification; // For NotificationDto

// --- DTOs specific to Notifications ---
// (Define these in kalamon_University/DTOs/Notification/)
// namespace kalamon_University.DTOs.Notification;
// public record NotificationDto(int Id, string Message, DateTime DateSent, bool IsRead, string? RelatedEntityType, int? RelatedEntityId);
// public record CreateNotificationDto(Guid TargetUserId, string Message, string? RelatedEntityType = null, int? RelatedEntityId = null);

namespace kalamon_University.Core.Interfaces.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using kalamon_University.DTOs.Common;
using kalamon_University.DTOs.Notification;

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