using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace kalamon_University.StudentPortalDtos;

public record StudentProfileDto(
    Guid StudentId, // Student.Id
    Guid Id, //User.Id
    string Name,
    string Email
// يمكنك إضافة أي معلومات أخرى خاصة بملف الطالب ترغب بعرضها
// مثلاً: string? Department, DateTime? EnrollmentDate, string? Major
);

public class UpdateStudentProfileDto // Using class for potential future complexity or if records feel too restrictive
{
    // الطالب يمكنه تعديل بعض معلوماته الأساسية
    [StringLength(100, ErrorMessage = "name cannot exceed 100 characters.")]
    public string? Name { get; set; }

    // لا يُسمح للطالب عادةً بتغيير الإيميل الرئيسي أو رقم الطالب الجامعي مباشرة
}

public record EnrolledCourseDto(
    int CourseId,
    string CourseName,
    string ProfessorName,
    int AbsenceCount,
    int MaxAbsenceLimit,
    bool IsExcluded // هل تم حرمان الطالب من هذا الكورس
                    // يمكنك إضافة تفاصيل أخرى مثل: int Credits, string? Grade
);

public record CourseAttendanceDetailsDto(
public record CourseAttendanceDetailsDto(
    int CourseId,
    string CourseName,
    DateTime SessionDate,
    bool IsPresent,
    string? Notes // ملاحظات الدكتور على الحضور (إن وجدت)
);