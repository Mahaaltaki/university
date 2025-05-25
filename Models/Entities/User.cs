using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using kalamon_University.Models.Entities;
using Microsoft.AspNetCore.Identity;
namespace kalamon_University.Models.Entities
{
    public class User : IdentityUser<Guid>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public  string Email { get; set; }
        [Required]
        public  string PasswordHash { get; set; }
        public Role Role { get; set; } = Role.Student;

        /* الطالب يتم تعريفه في جدول Students وله مفتاح أجنبي يشير إلى UserID.
        الأستاذ كذلك يُخزن في جدول Professors وله مفتاح أجنبي يشير إلى UserID

       User يمكن أن يكون إما مرتبطًا بسجل في Students أو سجل في Professors.

    العلاقة اختيارية وليست إلزامية، لذا استخدمنا ? بعد Student وProfessor.
        */
        public Student? Student { get; set; }
        public Professor? Professor { get; set; }
        //إذا كان دكتور 
        public virtual
        ICollection<Course> TaughtCourses
        { get; set; = new List<Course>();
        //إذا كان طالب
        public virtual ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Warning> WarningsReceived { get; set; } = new List<Warning>();

    }
}
