using System;
using System.ComponentModel.DataAnnotations;

namespace kalamon_University.DTOs.Notification

public record NotificationDto(
    int Id,
    Guid TargetUserId,
    string Message,
    DateTime DateSent,
    bool IsRead,
    string? RelatedEntityType, // e.g., "Warning", "CourseAnnouncement", "System"
    int? RelatedEntityId,   // e.g., WarningId, CourseId
    string? Link // رابط اختياري للانتقال إليه عند الضغط على الإشعار
);

public record CreateNotificationDto
{
    [Required]
    public Guid TargetUserId { get; set; } // ApplicationUser.Id للمستخدم المستهدف

    [Required]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Notification message must be between 5 and 1000 characters.")]
    public string Message { get; set; }

    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
    public string? Link { get; set; }
}

public record MarkNotificationAsReadDto
{
    [Required]
    public int NotificationId { get; set; }
}