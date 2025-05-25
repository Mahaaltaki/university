using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using kalamon_University.Models.Entities;

namespace kalamon_University.Models.Entities
{
    public class Attendance
    {
        public int Id { get; set; } // PK
        [ForeignKey("StudentId")]
        public Guid StudentId { get; set; } // FK
    
        public virtual Student Student { get; set; }
        [ForeignKey("CourseId")]
        public int CourseId { get; set; } // FK
    
        public virtual Course Course { get; set; }

        public DateTime SessionDate { get; set; }
        public bool IsPresent { get; set; }
        public string? Notes { get; set; } // ملاحظات إذا وجدت
    }
}
