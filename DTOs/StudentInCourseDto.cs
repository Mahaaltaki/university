using System;

// تأكد أن مساحة الاسم (namespace) صحيحة وتطابق ما تستخدمه في الواجهة
namespace kalamon_University.DTOs.Course
{
    public class StudentInCourseDto
    {
        public Guid StudentId { get; set; } // هوية الطالب
        public string FullName { get; set; } // اسم الطالب الكامل
        public string Email { get; set; } // إيميل الطالب
        public DateTime EnrollmentDate { get; set; } // تاريخ تسجيله في الكورس
        // يمكنك إضافة أي بيانات أخرى تحتاجها من كائن الطالب أو المستخدم
    }
}