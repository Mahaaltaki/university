using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kalamon_University.Models.Entities
{
    public class Student
    {
        [Key]
        [ForeignKey("User")] // يشير إلى الخاصية الملاحية "User" أدناه
        public Guid UserId { get; set; } // PK & FK

        //العلاقة "طالب ينتمي إلى مستخدم"
        public virtual User User { get; set; } = null!;

        // علاقة واحد إلى متعدد (One-to-Many): طالب يمكنه الالتحاق بعدة مواد
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        // علاقة one-to-many مع Attendance
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        // علاقة one-to-many مع Warning
        public virtual ICollection<Warning> Warnings { get; set; } = new List<Warning>();
    }
}