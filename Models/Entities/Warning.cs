using System.ComponentModel.DataAnnotations.Schema;
using kalamon_University.Models.Enums;
namespace kalamon_University.Models.Entities
{

    
    public class Warning
    {
        public int Id { get; set; } // PK
        public Guid StudentId { get; set; } // FK
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        public int CourseId { get; set; } // FK (الإنذار مرتبط بكورس معين)
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public WarningType Type { get; set; }
        public string Message { get; set; }
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;
        public Guid? IssuedByUserId { get; set; } // من أصدر الإنذار (دكتور أو أدمن)
        [ForeignKey("IssuedByUserId")]
        public virtual User? IssuedByUser { get; set; }
    }
}