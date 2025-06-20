using System.ComponentModel.DataAnnotations;
namespace kalamon_University.DTOs.ProfessorPortal
{   
    public class SendNotificationDto
        {
            [Required]
            [StringLength(500, MinimumLength = 10)]
            public string Message { get; set; } = string.Empty;
        }
}

