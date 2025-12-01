using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a report of an inappropriate review
    /// </summary>
    public class ReviewReport
    {
        /// <summary>
        /// Unique identifier for the report
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the review being reported
        /// </summary>
        public Guid ReviewId { get; set; }

        /// <summary>
        /// Navigation property to the reported review
        /// </summary>
        [ForeignKey("ReviewId")]
        public virtual Review Review { get; set; } = null!;

        /// <summary>
        /// ID of the user who reported the review
        /// </summary>
        public Guid ReporterId { get; set; }

        /// <summary>
        /// Reason for reporting the review
        /// </summary>
        [Required]
        [MaxLength(500, ErrorMessage = "Report reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the report was submitted
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ID of the admin who handled the report (if handled)
        /// </summary>
        public Guid? HandledById { get; set; }

        /// <summary>
        /// Result of the moderation action (approved, rejected, etc.)
        /// </summary>
        public string? ModerationResult { get; set; }

        /// <summary>
        /// Date and time when the report was handled (if handled)
        /// </summary>
        public DateTime? HandledAt { get; set; }
    }
}