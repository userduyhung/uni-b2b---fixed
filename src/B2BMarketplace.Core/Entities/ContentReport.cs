using System.ComponentModel.DataAnnotations;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a content report submitted by a user
    /// </summary>
    public class ContentReport
    {
        /// <summary>
        /// Unique identifier for the content report
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the reported content entity
        /// </summary>
        public Guid ReportedContentId { get; set; }

        /// <summary>
        /// Type of content being reported
        /// </summary>
        public Enums.ContentType ContentType { get; set; }

        /// <summary>
        /// Foreign key to the User who reported the content
        /// </summary>
        public Guid ReportedById { get; set; }

        /// <summary>
        /// Reason for reporting the content
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of why the content is being reported
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Status of the report
        /// </summary>
        public Enums.ReportStatus Status { get; set; }

        /// <summary>
        /// Foreign key to the User who resolved the report (if resolved)
        /// </summary>
        public Guid? ResolvedById { get; set; }

        /// <summary>
        /// Timestamp when the content was reported
        /// </summary>
        public DateTime ReportedAt { get; set; }

        /// <summary>
        /// Timestamp when the report was resolved (if resolved)
        /// </summary>
        public DateTime? ResolvedAt { get; set; }

        /// <summary>
        /// Action taken during moderation (if resolved)
        /// </summary>
        public Enums.ModerationAction? ActionTaken { get; set; }

        /// <summary>
        /// Navigation property to the User who reported the content
        /// </summary>
        public User ReportedBy { get; set; } = null!;

        /// <summary>
        /// Navigation property to the User who resolved the report (if resolved)
        /// </summary>
        public User? ResolvedBy { get; set; }

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public ContentReport()
        {
            Id = Guid.NewGuid();
            ReportedAt = DateTime.UtcNow;
            Status = Enums.ReportStatus.Pending;
        }
    }
}