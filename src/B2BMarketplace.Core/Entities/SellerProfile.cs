using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a seller's profile information
    /// </summary>
    public class SellerProfile
    {
        /// <summary>
        /// Unique identifier for the seller profile
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the User entity
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Seller's company name
        /// </summary>
        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Legal representative of the company
        /// </summary>
        [Required]
        [StringLength(255)]
        public string LegalRepresentative { get; set; } = string.Empty;

        /// <summary>
        /// Company tax identification number
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TaxId { get; set; } = string.Empty;

        /// <summary>
        /// Industry sector of the company
        /// </summary>
        [StringLength(100)]
        public string? Industry { get; set; }

        /// <summary>
        /// Seller's country
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Company description
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Whether the seller has been verified by admin
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Whether the seller has premium subscription
        /// </summary>
        public bool IsPremium { get; set; }

        /// <summary>
        /// Whether the seller has a verified badge (distinguished from basic verification)
        /// </summary>
        public bool HasVerifiedBadge { get; set; }

        /// <summary>
        /// Timestamp when the seller became premium (nullable for backward compatibility)
        /// </summary>
        public DateTime? PremiumSince { get; set; }

        /// <summary>
        /// Business name of the seller (may differ from company name)
        /// </summary>
        [StringLength(255)]
        public string? BusinessName { get; set; }

        /// <summary>
        /// Primary category for the seller's business
        /// </summary>
        public Guid? PrimaryCategoryId { get; set; }

        /// <summary>
        /// Navigation property to the primary category
        /// </summary>
        [ForeignKey("PrimaryCategoryId")]
        public ProductCategory? PrimaryCategory { get; set; }

        /// <summary>
        /// Average rating of the seller
        /// </summary>
        public double? AverageRating { get; set; }

        /// <summary>
        /// Total number of ratings received by the seller
        /// </summary>
        public int NumberOfRatings { get; set; }

        /// <summary>
        /// Timestamp when profile was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when profile was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Updated date (alias for UpdatedAt)
        /// </summary>
        public DateTime UpdatedDate => UpdatedAt;

        /// <summary>
        /// Navigation property to the User entity
        /// </summary>
        [ForeignKey("UserId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public User User { get; set; } = null!;

        /// <summary>
        /// Navigation property to certifications
        /// </summary>
        public List<Certification> Certifications { get; set; } = new();

        /// <summary>
        /// Navigation property to products
        /// </summary>
        public List<Product> Products { get; set; } = new();

        /// <summary>
        /// Navigation property to RFQ recipients
        /// </summary>
        public List<RFQRecipient> RFQRecipients { get; set; } = new();

        /// <summary>
        /// Navigation property to quotes
        /// </summary>
        public List<Quote> Quotes { get; set; } = new();

        /// <summary>
        /// Navigation property to reviews received by this seller
        /// </summary>
        public List<Review> ReviewsReceived { get; set; } = new();

        /// <summary>
        /// Navigation property to premium subscriptions
        /// </summary>
        public List<PremiumSubscription> PremiumSubscriptions { get; set; } = new();

        /// <summary>
        /// Navigation property to review replies made by this seller
        /// </summary>
        public List<ReviewReply> ReviewReplies { get; set; } = new();

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public SellerProfile()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsVerified = false;
            IsPremium = false;
            HasVerifiedBadge = false;
        }
    }
}
