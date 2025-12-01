using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for buyer profile
    /// </summary>
    public class BuyerProfileDto
    {
        /// <summary>
        /// Buyer's full name
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Buyer's company name
        /// </summary>
        [StringLength(255)]
        public string? CompanyName { get; set; }

        /// <summary>
        /// Buyer's country
        /// </summary>
        [StringLength(100)]
        public string? Country { get; set; }

        /// <summary>
        /// Buyer's phone number
        /// </summary>
        [StringLength(20)]
        public string? Phone { get; set; }
    }

    /// <summary>
    /// Data transfer object for seller profile
    /// </summary>
    public class SellerProfileDto
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
        /// Business name of the seller (may differ from company name)
        /// </summary>
        [StringLength(255)]
        public string? BusinessName { get; set; }

        /// <summary>
        /// Primary category ID for the seller's business
        /// </summary>
        public Guid? PrimaryCategoryId { get; set; }
    }

    /// <summary>
    /// Data transfer object for updating buyer profile
    /// </summary>
    public class UpdateBuyerProfileDto
    {
        /// <summary>
        /// Buyer's full name
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Buyer's company name
        /// </summary>
        [StringLength(255)]
        public string? CompanyName { get; set; }

        /// <summary>
        /// Buyer's country
        /// </summary>
        [StringLength(100)]
        public string? Country { get; set; }

        /// <summary>
        /// Buyer's phone number
        /// </summary>
        [StringLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// Buyer's industry
        /// </summary>
        [StringLength(100)]
        public string? Industry { get; set; }

        /// <summary>
        /// Buyer's description
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Buyer's website
        /// </summary>
        [StringLength(255)]
        public string? Website { get; set; }

        /// <summary>
        /// Buyer's city
        /// </summary>
        [StringLength(100)]
        public string? City { get; set; }
    }

    /// <summary>
    /// Data transfer object for updating seller profile
    /// </summary>
    public class UpdateSellerProfileDto
    {
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
        /// Seller's website
        /// </summary>
        [StringLength(255)]
        public string? Website { get; set; }

        /// <summary>
        /// Seller's city
        /// </summary>
        [StringLength(100)]
        public string? City { get; set; }
    }
}