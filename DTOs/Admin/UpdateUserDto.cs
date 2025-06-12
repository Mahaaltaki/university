
using System;
using System.ComponentModel.DataAnnotations;

namespace kalamon_University.DTOs.Admin;
public class UpdateUserDto
{
    // معلومات User
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }
    
    public string? FullName { get; set; }
    public bool? EmailConfirmed { get; set; }
    public bool? LockoutEnabled { get; set; } // للتحكم في قفل الحساب
    public DateTimeOffset? LockoutEnd { get; set; } // لتحديد تاريخ انتهاء القفل (null لإلغاء القفل)
    // الأدمن قد يحتاج لتغيير كلمة المرور، ولكن هذا يتطلب DTO خاص أو معامل منفصل للأمان
    // public string? NewPassword { get; set; }

    // الأدمن قد يحتاج لتغيير الأدوار (هذا يتطلب منطق خاص في السيرفيس)
    // public List<string>? Roles { get; set; }


    // معلومات خاصة بالطالب (تُرسل فقط إذا كان المستخدم طالبًا ويُراد تعديلها)
    public string? StudentId { get; set; }
    // public string? StudentDepartment { get; set; }


    // معلومات خاصة بالدكتور (تُرسل فقط إذا كان المستخدم دكتورًا ويُراد تعديلها)
    public string? ProfessorId { get; set; }
    public string? Specialization { get; set; }
    // public string? DoctorOfficeNumber { get; set; }

    // ملاحظة: عند التعديل، يجب أن يعرف الـ Service Layer نوع المستخدم (طالب/دكتور)
    // ليقوم بتحديث الجدول الصحيح (Students أو professor  إلى User.
    // يمكن تمرير UserType كـ query parameter أو استنتاجه من الأدوار.
};