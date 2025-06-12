// File: kalamon_University/DTOs/ProfessorPortal/UpdateProfessorProfileDto.cs
using System.ComponentModel.DataAnnotations;

namespace kalamon_University.DTOs.ProfessorPortal
{
    public record UpdateProfessorProfileDto
    {
        // يمكن للأستاذ تحديث تخصصه (أو الأدمن فقط، حسب قواعد العمل)
        [StringLength(100, ErrorMessage = "Specialization must be up to 100 characters.")]
        public string? Specialization { get; set; } // لتحديث Professor.Specialization

        
        
    }
}