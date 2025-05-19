using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using kalamon_University.Models.Entities;

namespace kalamon_University.Models.Entities
{
    public class Notification
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int SenderID { get; set; }

        [Required]
        public int ReceiverID { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(SenderID))]
        public User Sender { get; set; } = null!;

        [ForeignKey(nameof(ReceiverID))]
        public User Receiver { get; set; } = null!;
    }
}
