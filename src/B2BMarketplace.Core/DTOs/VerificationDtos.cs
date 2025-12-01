using System.ComponentModel.DataAnnotations;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for pending verification requests
    /// </summary>
    public class PendingVerificationDto
    {
        /// <summary>
        /// Certification ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Seller ID
        /// </summary>
        public Guid SellerId { get; set; }

        /// <summary>
        /// Seller's name
        /// </summary>
        public string SellerName { get; set; } = string.Empty;

        /// <summary>
        /// Seller's company name
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when certification was submitted
        /// </summary>
        public DateTime SubmittedAt { get; set; }

        /// <summary>
        /// Name of the certification
        /// </summary>
        public string CertificationName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for detailed verification request
    /// </summary>
    public class VerificationDetailsDto
    {
        /// <summary>
        /// Certification ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Seller ID
        /// </summary>
        public Guid SellerId { get; set; }

        /// <summary>
        /// Seller's name
        /// </summary>
        public string SellerName { get; set; } = string.Empty;

        /// <summary>
        /// Seller's company name
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Legal representative of the company
        /// </summary>
        public string LegalRepresentative { get; set; } = string.Empty;

        /// <summary>
        /// Company tax identification number
        /// </summary>
        public string TaxId { get; set; } = string.Empty;

        /// <summary>
        /// Industry sector of the company
        /// </summary>
        public string? Industry { get; set; }

        /// <summary>
        /// Seller's country
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Company description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Name of the certification
        /// </summary>
        public string CertificationName { get; set; } = string.Empty;

        /// <summary>
        /// Path to the certification document
        /// </summary>
        public string? DocumentPath { get; set; }

        /// <summary>
        /// Timestamp when certification was submitted
        /// </summary>
        public DateTime SubmittedAt { get; set; }

        /// <summary>
        /// Current status of the certification
        /// </summary>
        public CertificationStatus Status { get; set; }
    }

    /// <summary>
    /// Data transfer object for verification decision
    /// </summary>
    public class VerificationDecisionDto
    {
        /// <summary>
        /// Notes from admin reviewer (required for rejection)
        /// </summary>
        [StringLength(1000)]
        public string? AdminNotes { get; set; }
    }

    /// <summary>
    /// Data transfer object for manual verification
    /// </summary>
    public class ManualVerificationDto
    {
        /// <summary>
        /// Whether the seller should be marked as verified
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Optional notes from admin
        /// </summary>
        [StringLength(1000)]
        public string? AdminNotes { get; set; }
    }

    /// <summary>
    /// Data transfer object for verification summary
    /// </summary>
    public class VerificationSummaryDto
    {
        /// <summary>
        /// Seller profile ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Seller's company name
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Whether the seller is verified
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Whether the seller has premium status
        /// </summary>
        public bool IsPremium { get; set; }

        /// <summary>
        /// Number of approved certifications
        /// </summary>
        public int ApprovedCertificationsCount { get; set; }

        /// <summary>
        /// Number of pending certifications
        /// </summary>
        public int PendingCertificationsCount { get; set; }

        /// <summary>
        /// Last updated timestamp
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}