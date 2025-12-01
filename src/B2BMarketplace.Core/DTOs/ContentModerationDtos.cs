using System.ComponentModel.DataAnnotations;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for ContentReport entity
    /// </summary>
    public class ContentReportDto
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
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of why the content is being reported
        /// </summary>
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
    }

    /// <summary>
    /// Data transfer object for creating a new ContentReport
    /// </summary>
    public class CreateReportDto
    {
        /// <summary>
        /// Foreign key to the reported content entity
        /// </summary>
        [Required]
        public Guid ReportedContentId { get; set; }

        /// <summary>
        /// Type of content being reported
        /// </summary>
        [Required]
        public Enums.ContentType ContentType { get; set; }

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
    }

    /// <summary>
    /// Data transfer object for resolving a ContentReport
    /// </summary>
    public class ModerationResolutionDto
    {
        /// <summary>
        /// Action taken during moderation
        /// </summary>
        [Required]
        public Enums.ModerationAction Action { get; set; }

        /// <summary>
        /// Notes about the moderation action
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Data transfer object for filtering ContentReports
    /// </summary>
    public class ReportFilterDto
    {
        /// <summary>
        /// Filter by report status
        /// </summary>
        public Enums.ReportStatus? Status { get; set; }

        /// <summary>
        /// Filter by content type
        /// </summary>
        public Enums.ContentType? ContentType { get; set; }

        /// <summary>
        /// Page number for pagination
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Page size for pagination
        /// </summary>
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Data transfer object for repeat offenders
    /// </summary>
    public class RepeatOffenderDto
    {
        /// <summary>
        /// Foreign key to the User entity
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Email of the user
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Number of resolved reports against this user
        /// </summary>
        public int ReportCount { get; set; }

        /// <summary>
        /// Timestamp of the last report
        /// </summary>
        public DateTime LastReportedAt { get; set; }
    }
}