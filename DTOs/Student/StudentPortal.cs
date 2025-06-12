// In DTOs/StudentPortal/
using System;

namespace kalamon_University.DTOs.Student
{
    public record StudentProfileDto(
        Guid UserId,
        string FullName,
        string Email,
        string? UserName,
        bool EmailConfirmed,
        Guid StudentEntityId, // PK of Student table
        string StudentIdNumber
    );

    //public record UpdateStudentProfileDto
    //{
    //   // public string? PhoneNumber { get; set; }
    //    // أي حقول أخرى يمكن للطالب تحديثها
    //}

    public record EnrolledCourseDto(
        int CourseId,
        string CourseName,
        string CourseCode,
        string? ProfessorName,
        int TotalAbsencesInCourse,
        int MaxAbsenceLimitForCourse
    );

    public record CourseAttendanceDetailsDto(
        int CourseId,
        string CourseName,
        DateTime SessionDate,
        bool IsPresent,
        string? Notes
    );
}