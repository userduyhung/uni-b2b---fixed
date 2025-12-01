using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; } = string.Empty;
    }

    public class CreateNotificationDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public Guid? RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; } = string.Empty;
    }

    public class UpdateNotificationDto
    {
        public bool IsRead { get; set; }
    }

    public class NotificationPreferencesDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool EmailNotificationsEnabled { get; set; }
        public bool RFQResponseNotifications { get; set; }
        public bool RFQReceivedNotifications { get; set; }
        public bool QuoteUpdatedNotifications { get; set; }
        public bool CertificationStatusNotifications { get; set; }
        public bool AccountStatusNotifications { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateNotificationPreferencesDto
    {
        public bool EmailNotificationsEnabled { get; set; }
        public bool RFQResponseNotifications { get; set; }
        public bool RFQReceivedNotifications { get; set; }
        public bool QuoteUpdatedNotifications { get; set; }
        public bool CertificationStatusNotifications { get; set; }
        public bool AccountStatusNotifications { get; set; }
    }

    public class NotificationListDto
    {
        public IEnumerable<NotificationDto> Notifications { get; set; } = new List<NotificationDto>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public enum NotificationType
    {
        RFQResponse,
        RFQReceived,
        QuoteUpdated,
        CertificationApproved,
        CertificationRejected,
        AccountLocked,
        AccountUnlocked,
        RFQClosed,
        Review,
        Payment
    }
}