using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityApi.Core.Entities;

public class Notification
{
    public int Id { get; set; }
    [ForeignKey("TargetUserId")]
    public Guid TargetUserId { get; set; } // FK to ApplicationUser (الطالب أو الدكتور المستهدف)

    public virtual User TargetUser { get; set; }
    public string Message { get; set; }
    public DateTime DateSent { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    public string? RelatedEntityType { get; set; } // e.g., "Warning", "Course"
    public int? RelatedEntityId { get; set; }   // e.g., WarningId, CourseId
}