using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kalamon_University.Models.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

    
        [ForeignKey("Professor")] // يشير إلى خاصية التصفح "Professor"
        public Guid? ProfessorId { get; set; } // اسم الخاصية بحرف صغير في البداية للـ FK، وجعلها nullable

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
       
        [Range(0, 10, ErrorMessage = "Practical hours must be between 0 and 10.")]
        public int PracticalHours { get; set; }

        [Range(0, 10, ErrorMessage = "Theoretical hours must be between 0 and 10.")]
        public int TheoreticalHours { get; set; }

        // TotalHours: إذا كانت ستُحسب، يمكن إزالة الـ setter أو جعلها [NotMapped]
        // إذا كانت ستُدخل، الـ Range مناسب. سأفترض أنها ستُدخل.
        [Required(ErrorMessage = "Total hours are required.")] // إذا كانت ستدخل
        [Range(0, 20, ErrorMessage = "Total hours must be between 0 and 20.")]
        public int TotalHours { get; set; }

       
        [Required(ErrorMessage = "Max absence limit is required.")]
        [Range(1, 20, ErrorMessage = "Max absence limit must be between 1 and 20.")]
        public int MaxAbsenceLimit { get; set; } = 5; // القيمة الافتراضية جيدة


        // بافتراض أن "Professor" هو في الواقع كائن "User" له دور "Professor".
        // إذا كان لديك كيان Professor منفصل، احتفظ بـ Professor? وتأكد من أن ForeignKey صحيح.
        public Professor? Professor { get; set; }
        // علاقة many-to-many مع الطلاب من خلال جدول وسيط Enrollment
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        // الإنذارات المرتبطة بالكورس
        public virtual ICollection<Warning> Warnings { get; set; } = new List<Warning>();

        public ICollection<Course>? TaughtCourses { get; set; } = new List<Course>();
        


        // [NotMapped]
        // public int CalculatedTotalHours => PracticalHours + TheoreticalHours;
        // يمكنك تفعيل هذا إذا كنت تريد حسابها ديناميكيًا بدلاً من تخزينها.
        // إذا قمت بتفعيلها، قد ترغب في جعل خاصية TotalHours للقراءة فقط أو إزالتها من قاعدة البيانات.
        // للتبسيط، سأبقي TotalHours كحقل يتم إدخاله كما هو في الـ DTOs.
    }
}