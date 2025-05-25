using System.ComponentModel.DataAnnotations.Schema;
using kalamon_University.Models.Entities;

namespace kalamon_University.Models.Entities
{
    public class Enrollments
    {
        [ForeignKey("StudentId")]
        public Guid StudentId { get; set; } // Composite PK, FK
        
        public virtual Student Student { get; set; }

        public int CourseId { get; set; }  // Composite PK, FK
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
     
    }
}
