using System.ComponentModel.DataAnnotations;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents an audit log entry for content moderation actions
    /// </summary>
    public class ModerationAuditLog
    {
        /// <summary>
        /// Unique identifier for the audit log entry
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the ContentReport entity
        /// </summary>
        public Guid ContentReportId { get; set; }

        /// <summary>
        /// Foreign key to the User who performed the moderation action
        /// </summary>
        public Guid ModeratedById { get; set; }

        /// <summary>
        /// Action taken during moderation
        /// </summary>
        public Enums.ModerationAction Action { get; set; }

        /// <summary>
        /// Notes about the moderation action
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Timestamp when the moderation action was performed
        /// </summary>
        public DateTime ModeratedAt { get; set; }

        /// <summary>
        /// Navigation property to the ContentReport entity
        /// </summary>
        public ContentReport ContentReport { get; set; } = null!;

        /// <summary>
        /// Navigation property to the User who performed the moderation action
        /// </summary>
        public User ModeratedBy { get; set; } = null!;

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public ModerationAuditLog()
        {
            Id = Guid.NewGuid();
            ModeratedAt = DateTime.UtcNow;
        }
    }
}