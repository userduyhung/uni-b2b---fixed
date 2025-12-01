using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public NotificationType Type { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }

        public Guid? RelatedEntityId { get; set; }

        [StringLength(50)]
        public string RelatedEntityType { get; set; } = string.Empty;
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