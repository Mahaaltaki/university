using System;
using System.ComponentModel.DataAnnotations;
using kalamon_University.Models.Entities;
namespace kalamon_University.DTOs.Notification
{

    public class NotificationDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
       
       
    }

}