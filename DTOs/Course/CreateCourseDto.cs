
using System;
using System.ComponentModel.DataAnnotations;

namespace kalamon_University.DTOs.Course
{
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Course name is required.")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

       

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Practical hours are required.")]
        [Range(0, 10)]
        public int PracticalHours { get; set; }

        [Required(ErrorMessage = "Theoretical hours are required.")]
        [Range(0, 10)]
        public int TheoreticalHours { get; set; }

        // TotalHours: يمكن حسابها في الخدمة أو إرسالها من الـ client
        // إذا كانت ستحسب في الخدمة، يمكن إزالتها من هنا.
        // إذا كانت سترسل، اجعلها مطلوبة.
        [Required(ErrorMessage = "Total hours are required.")]
        [Range(0, 20)] // يجب أن يكون المجموع منطقيًا
        public int TotalHours { get; set; }

        [Required(ErrorMessage = "Max absence limit is required.")]
        [Range(1, 20)]
        public int MaxAbsenceLimit { get; set; } = 5;

        public Guid ProfessorId { get; set; } // ، FK لـ User.Id للأستاذ
    
    }
}