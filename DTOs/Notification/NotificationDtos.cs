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
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? RelatedEntityName { get; set; }

        public NotificationDto(Models.Entities.Notification entity)
        {
            Id = entity.Id;
            UserId = entity.UserId;
            Message = entity.Message;
            CreatedAt = entity.CreatedAt;
            IsRead = entity.IsRead;
            RelatedEntityType = entity.RelatedEntityType;
            RelatedEntityId = entity.RelatedEntityId;
            RelatedEntityName = entity.RelatedEntityName;
        }
    }

}