using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using kalamon_University.Models.Entities;
using System;

namespace kalamon_University.Models.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Professor")]
        public Guid ProfessorID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int PracticalHours { get; set; }
        public int TheoreticalHours { get; set; }
        public int TotalHours { get; set; }
        public int MaxAbsenceLimit { get; set; } = 5; // مثال: الحد الأقصى للغياب

        // البروفيسور الذي يعطي هذا الكورس
        public Professor Professor { get; set; } = null!;

        // علاقة many-to-many مع الطلاب
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        // الحضور المرتبط بهذا الكورس
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        // الإنذارات المرتبطة بالكورس
        public virtual ICollection<Warning> Warnings { get; set; } = new List<Warning>();
    }
}
