using System.ComponentModel.DataAnnotations;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a seller certification that requires admin approval
    /// </summary>
    public class Certification
    {
        /// <summary>
        /// Unique identifier for the certification
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the SellerProfile entity
        /// </summary>
        public Guid SellerProfileId { get; set; }

        /// <summary>
        /// Name of the certification (e.g., ISO 9001)
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Path to the uploaded document
        /// </summary>
        [StringLength(500)]
        public string? DocumentPath { get; set; }

        /// <summary>
        /// Approval status of the certification
        /// </summary>
        public CertificationStatus Status { get; set; }

        /// <summary>
        /// Timestamp when certification was submitted
        /// </summary>
        public DateTime SubmittedAt { get; set; }

        /// <summary>
        /// Timestamp when certification was reviewed
        /// </summary>
        public DateTime? ReviewedAt { get; set; }

        /// <summary>
        /// Notes from admin reviewer
        /// </summary>
        [StringLength(1000)]
        public string? AdminNotes { get; set; }

        /// <summary>
        /// Navigation property to the SellerProfile entity
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public SellerProfile SellerProfile { get; set; } = null!;

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public Certification()
        {
            Id = Guid.NewGuid();
            SubmittedAt = DateTime.UtcNow;
            Status = CertificationStatus.Pending;
        }
    }
}