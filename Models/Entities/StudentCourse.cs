using System.ComponentModel.DataAnnotations.Schema;
using kalamon_University.Models.Entities;

namespace kalamon_University.Models.Entities
{
    public class StudentCourse
    {
        [ForeignKey("Student")]
        public int StudentID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }

        public Student Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
