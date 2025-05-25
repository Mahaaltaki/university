using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kalamon_University.Models.Entities
{
    public class Professor
    {
        public Guid Id { get; set; } // PK

        [Key, ForeignKey("User")]
        public Guid UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        // علاقة 1:1 مع User
        public User User { get; set; } ;

        // علاقة 1:N مع Course
        public virtual ICollection<Course> TaughtCourses { get; set; } = new List<Course>();
    }
}
