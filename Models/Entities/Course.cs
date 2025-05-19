using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using kalamon_University.Models.Entities;

namespace kalamon_University.Models.Entities
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }

        [ForeignKey("Professor")]
        public int ProfessorID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int PracticalHours { get; set; }
        public int TheoreticalHours { get; set; }
        public int TotalHours { get; set; }

        // البروفيسور الذي يعطي هذا الكورس
        public Professor Professor { get; set; } = null!;

        // علاقة many-to-many مع الطلاب
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

        // الحضور المرتبط بهذا الكورس
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        // الإنذارات المرتبطة بالكورس
        public ICollection<Warning> Warnings { get; set; } = new List<Warning>();
    }
}
