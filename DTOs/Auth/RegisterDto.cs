using System.ComponentModel.DataAnnotations;

namespace kalamon_University.DTOs.Auth
{
    public class RegisterDto // يمكن إعادة تسميته ليتوافق مع AuthService
    {
     
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)] // أضف تحققًا أساسيًا لكلمة المرور
        public string Password { get; set; }

        [Required]
        public string RoleName { get; set; } // e.g., "Student", "Professor", "Admin"

        
        public string? Specialization { get; set; }  // إذا كان الدور "Professor"
    }
}