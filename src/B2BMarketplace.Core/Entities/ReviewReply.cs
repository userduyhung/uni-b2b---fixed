using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a reply from a seller to a review left by a buyer
    /// </summary>
    public class ReviewReply
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ReviewId { get; set; }

        [Required]
        public Guid SellerProfileId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ReplyContent { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Review? Review { get; set; }
        public virtual SellerProfile? SellerProfile { get; set; }
    }
}