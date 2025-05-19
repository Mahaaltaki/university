using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using kalamon_University.Models.Entities;

namespace kalamon_University.Models.Entities
{
    public class Warning
    {
        [Key]
        public int WarningID { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }

        [Required]
        public string Cause { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public Student Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
