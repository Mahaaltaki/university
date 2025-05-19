using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kalamon_University.Models.Entities
{
    public class Professor
    {
        [Key, ForeignKey("User")]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        // علاقة 1:1 مع User
        public User User { get; set; } = null!;

        // علاقة 1:N مع Course
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
