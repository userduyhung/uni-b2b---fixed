using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Junction table for ContentItem and ContentTag many-to-many relationship
    /// </summary>
    public class ContentItemTag
    {
        [Required]
        public Guid ContentItemId { get; set; }

        [Required]
        public Guid ContentTagId { get; set; }

        // Navigation properties
        [ForeignKey("ContentItemId")]
        public virtual ContentItem? ContentItem { get; set; }

        [ForeignKey("ContentTagId")]
        public virtual ContentTag? ContentTag { get; set; }
    }
}