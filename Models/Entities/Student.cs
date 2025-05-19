using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using kalamon_University.Models.Entities;

namespace kalamon_University.Models.Entities
{
    public class Student
    {
        [ForeignKey("User")]
        public int UserID { get; set; }

        public User User { get; set; } = null!;

        // علاقة many-to-many مع Course عبر StudentCourse
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

        // علاقة one-to-many مع Attendance
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        // علاقة one-to-many مع Warning
        public ICollection<Warning> Warnings { get; set; } = new List<Warning>();
    }
}
