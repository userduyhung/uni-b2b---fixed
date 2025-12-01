using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a category for organizing content
    /// </summary>
    public class ContentCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Slug { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid? CreatedById { get; set; }

        public Guid? UpdatedById { get; set; }

        // Navigation property
        public virtual ICollection<ContentItem> ContentItems { get; set; } = new List<ContentItem>();
    }
}