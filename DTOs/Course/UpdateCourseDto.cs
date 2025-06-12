
using System;
using System.ComponentModel.DataAnnotations;

namespace kalamon_University.DTOs.Course
{
    public class UpdateCourseDto
    {
        [StringLength(100, MinimumLength = 3)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 10)]
        public int PracticalHours { get; set; }

        [Range(0, 10)]
        public int TheoreticalHours { get; set; }

        [Range(0, 20)]
        public int TotalHours { get; set; } // إذا كان يتم تحديثه

        [Range(1, 20)]
        public int MaxAbsenceLimit { get; set; }

        // لا نضع ProfessorUserId هنا، استخدم endpoint منفصل لتعيين/إلغاء تعيين الأستاذ
        public Guid ProfessorId { get; set; } // ، FK لـ User.Id للأستاذ

    }
}