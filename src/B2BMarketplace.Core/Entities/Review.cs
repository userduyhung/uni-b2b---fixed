using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a rating and review given by a buyer to a seller
    /// </summary>
    public class Review
    {
        /// <summary>
        /// Unique identifier for the review
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the buyer who submitted the review
        /// </summary>
        public Guid BuyerProfileId { get; set; }

        /// <summary>
        /// Navigation property to the buyer profile
        /// </summary>
        [ForeignKey("BuyerProfileId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual BuyerProfile BuyerProfile { get; set; } = null!;

        /// <summary>
        /// ID of the seller being reviewed
        /// </summary>
        public Guid SellerProfileId { get; set; }

        /// <summary>
        /// Navigation property to the seller profile
        /// </summary>
        [ForeignKey("SellerProfileId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual SellerProfile SellerProfile { get; set; } = null!;

        /// <summary>
        /// Rating value (1-5 stars)
        /// </summary>
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        /// <summary>
        /// Review comment/text
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Review comment cannot exceed 1000 characters")]
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the review was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Whether the review has been reported as inappropriate
        /// </summary>
        public bool IsReported { get; set; }

        /// <summary>
        /// Reason for reporting the review (if reported)
        /// </summary>
        [MaxLength(500, ErrorMessage = "Report reason cannot exceed 500 characters")]
        public string? ReportedReason { get; set; }

        /// <summary>
        /// Whether the review has been moderated and approved
        /// </summary>
        public bool IsApproved { get; set; } = true; // Default to approved unless reported and reviewed

        /// <summary>
        /// ID of the product being reviewed (optional - for product reviews)
        /// </summary>
        public Guid? ProductId { get; set; }

        /// <summary>
        /// Navigation property to the product being reviewed
        /// </summary>
        [ForeignKey("ProductId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Product? Product { get; set; }

        /// <summary>
        /// Navigation property to the seller's reply to this review
        /// </summary>
        public virtual ReviewReply? ReviewReply { get; set; }
    }
}