using System;

using System.ComponentModel.DataAnnotations;

namespace kalamon_University.DTOs.Course
//DTO لعرض معلومات الكورس (للقراءة فقط)
public record CourseDto(
    int Id,
    string Name,
    Guid ProfessorId;
public int PracticalHours { get; set; }
public int TheoreticalHours { get; set; }
public int TotalHours { get; set; }
int MaxAbsenceLimit,
    int EnrolledStudentsCount // Derived property
);
//يُستخدم عند إنشاء كورس جديد
public record CreateCourseDto(
    
    [Required(ErrorMessage = "Course name is required.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Course name must be between 5 and 100 characters.")]
    string Name,

    
    [Required(ErrorMessage = "Professor ID is required.")]
    Guid ProfessorId, // FK to ProfessorId (not ApplicationUser.Id)

    [Range(0, 30, ErrorMessage = "Max absence limit must be between 0 and 30.")]
    int MaxAbsenceLimit = 5 // Default value
);
//يُستخدم عند تحديث معلومات كورس، لذلك كل الحقول اختيارية
public record UpdateCourseDto(
    
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Course name must be between 5 and 100 characters.")]
    string? Name,

    
    Guid? ProfessorId,

    [Range(0, 30, ErrorMessage = "Max absence limit must be between 0 and 30.")]
    int? MaxAbsenceLimit
);
//يُستخدم لتسجيل طالب في كورس
public record EnrollStudentInCourseDto(
    [Required] int CourseId,
    [Required] Guid StudentId // The actual Student PK (Student.Id), not StudentIdNumber or ApplicationUser.Id
);
//يُستخدم لعرض معلومات طالب داخل كورس
public record StudentInCourseDto(
    Guid StudentId, // Student.Id
    Guid UserId, // ApplicationUser.Id for linking if needed
    string Name,
);
/*م تصميمه لاحتياجات مبسطة مثل:

تصدير بيانات إلى Excel
,طباعة قوائم بسيطة
 DTO for Excel generation, might also fit in DTOs/Excel/
*/
public record StudentBasicInfoDto(
    string StudentIdNumber, // University specific student ID
    string FullName
);
