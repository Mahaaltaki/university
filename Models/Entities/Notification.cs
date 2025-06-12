using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace kalamon_University.Models.Entities
{

    public class Notification
    {
        public int Id { get; set; }

        public Guid UserId { get; set; } // المفتاح الأجنبي للمستخدم المستهدف

        [ForeignKey("UserId")]
        public User TargetUser { get; set; } = null!; // خاصية التنقل إلى المستخدم المستلم

        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public string? RelatedEntityName { get; set; }
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
    }

}