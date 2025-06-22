using System;
using System.ComponentModel.DataAnnotations;

namespace kalamon_University.DTOs.Admin
{
    /// <summary>
    /// DTO يُستخدم لتمرير معرف الأستاذ عند تعيينه لكورس معين.
    /// </summary>
    public class AssignProfessorDto
    {
        [Required(ErrorMessage = "Professor user ID is required.")]
        public Guid ProfessorUserId { get; set; }
    }
}