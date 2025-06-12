using System;
using System.ComponentModel.DataAnnotations;

// تأكد أن مساحة الاسم (namespace) صحيحة وتطابق ما تستخدمه في الواجهة
namespace kalamon_University.DTOs.Course
{
    public class EnrollStudentInCourseDto
    {
        [Required]
        public Guid StudentId { get; set; } // هوية الطالب الذي سيتم تسجيله

        [Required]
        public int CourseId { get; set; } // هوية الكورس الذي سيسجل فيه
    }
}