using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a content item with enhanced management features
    /// </summary>
    public class ContentItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Excerpt { get; set; }

        [MaxLength(255)]
        public string? MetaTitle { get; set; }

        public string? MetaDescription { get; set; }

        [MaxLength(50)]
        public string ContentType { get; set; } = "page"; // 'page', 'blog', 'news', 'announcement'

        public Guid? CategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsPublished { get; set; } = false;

        public DateTime? PublishedAt { get; set; }

        public DateTime? ScheduledPublishAt { get; set; }

        public DateTime? ScheduledUnpublishAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual ContentCategory? Category { get; set; }

        public virtual ICollection<ContentItemTag> Tags { get; set; } = new List<ContentItemTag>();
    }
}