using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    public class NotificationPreferences
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public bool EmailNotificationsEnabled { get; set; } = true;

        public bool RFQResponseNotifications { get; set; } = true;

        public bool RFQReceivedNotifications { get; set; } = true;

        public bool QuoteUpdatedNotifications { get; set; } = true;

        public bool CertificationStatusNotifications { get; set; } = true;

        public bool AccountStatusNotifications { get; set; } = true;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}