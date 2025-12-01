using System.ComponentModel.DataAnnotations;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for public seller profile
    /// </summary>
    public class PublicSellerProfileDto
    {
        /// <summary>
        /// Unique identifier for the seller profile
        /// </summary>
        public Guid Id { get; set; }

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
        [StringLength(50)]
        public string? TaxId { get; set; }

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
        /// Seller's city
        /// </summary>
        [StringLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// Company address
        /// </summary>
        [StringLength(255)]
        public string? Address { get; set; }

        /// <summary>
        /// Company phone number
        /// </summary>
        [StringLength(50)]
        public string? Phone { get; set; }

        /// <summary>
        /// Company email
        /// </summary>
        [StringLength(255)]
        public string? Email { get; set; }

        /// <summary>
        /// Company website URL
        /// </summary>
        [StringLength(255)]
        public string? Website { get; set; }

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
        /// Average rating of the seller
        /// </summary>
        public double? AverageRating { get; set; }

        /// <summary>
        /// Total number of ratings received by the seller
        /// </summary>
        public int NumberOfRatings { get; set; }

        /// <summary>
        /// Recent reviews for the seller
        /// </summary>
        public List<ReviewDto> RecentReviews { get; set; } = new();

        /// <summary>
        /// Year the company was established
        /// </summary>
        public int? YearEstablished { get; set; }

        /// <summary>
        /// Number of employees in the company
        /// </summary>
        public string? CompanySize { get; set; }

        /// <summary>
        /// Business type (e.g., manufacturer, distributor, trading company)
        /// </summary>
        public string? BusinessType { get; set; }

        /// <summary>
        /// Number of years in business
        /// </summary>
        public int? YearsInBusiness { get; set; }

        /// <summary>
        /// Annual revenue range
        /// </summary>
        public string? AnnualRevenue { get; set; }

        /// <summary>
        /// Main markets served
        /// </summary>
        public string? MainMarkets { get; set; }

        /// <summary>
        /// Number of transactions completed
        /// </summary>
        public int? TotalTransactions { get; set; }

        /// <summary>
        /// Response time for inquiries (e.g., within 24 hours)
        /// </summary>
        public string? ResponseTime { get; set; }

        /// <summary>
        /// Response rate percentage
        /// </summary>
        public double? ResponseRate { get; set; }

        /// <summary>
        /// Explicit indicator for verified badge display
        /// </summary>
        public bool ShowVerifiedBadge => IsVerified;

        /// <summary>
        /// Text label for the verified badge
        /// </summary>
        public string VerifiedBadgeLabel => IsVerified ? "Verified" : string.Empty;

        /// <summary>
        /// CSS class for styling the verified badge
        /// </summary>
        public string VerifiedBadgeClass => IsVerified ? "badge-verified" : string.Empty;

        /// <summary>
        /// List of approved certifications for the seller
        /// </summary>
        public List<CertificationDto> Certifications { get; set; } = new();

        /// <summary>
        /// List of products for the seller
        /// </summary>
        public List<ProductDto> Products { get; set; } = new();
    }

    /// <summary>
    /// Result object for public seller profiles with pagination
    /// </summary>
    public class PublicSellerProfilesResult
    {
        /// <summary>
        /// List of public seller profiles
        /// </summary>
        public List<PublicSellerProfileDto> Profiles { get; set; } = new();

        /// <summary>
        /// Total count of public seller profiles
        /// </summary>
        public int TotalCount { get; set; }
    }
}