using System;
using System.ComponentModel.DataAnnotations;
using kalamon_University.Models.Entities; // For WarningType enum
using kalamon_University.Models.Enums;
namespace kalamon_University.DTOs.Warning
{

    public record WarningDto(
        int Id,
        int CourseId,
        string CourseName, // For display
        Guid StudentId,
        string StudentName, // For display
        WarningType Type,
        string Message,
        DateTime DateIssued,
        string? IssuedByUserName // اسم المستخدم الذي أصدر الإنذار (دكتور أو أدمن)
    );

    public record IssueWarningByProfessorDto // DTO للدكتور لإصدار إنذار
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public Guid StudentId { get; set; } // Student.Id
        [Required]
        public WarningType Type { get; set; } = WarningType.Other; // Default type
        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Warning message must be between 10 and 500 characters.")]
        public string Message { get; set; }
    }

    public record IssueWarningByAdminDto // DTO للأدمن لإصدار إنذار (قد يكون له صلاحيات أوسع)
    {
        [Required]
        public Guid TargetUserId { get; set; } // User.Id للطالب
        [Required]
        public int CourseId { get; set; } // الكورس المرتبط بالإنذار
        [Required]
        public WarningType Type { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Warning message must be between 10 and 500 characters.")]
        public string Message { get; set; }
    }





}
