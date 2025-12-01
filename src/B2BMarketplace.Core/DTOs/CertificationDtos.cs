using System.ComponentModel.DataAnnotations;
using B2BMarketplace.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for creating a certification
    /// </summary>
    public class CreateCertificationDto
    {
        /// <summary>
        /// Name of the certification (e.g., ISO 9001)
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Certification document file
        /// </summary>
        [Required]
        public IFormFile Document { get; set; } = null!;
    }

    /// <summary>
    /// Data transfer object for certification details
    /// </summary>
    public class CertificationDto
    {
        /// <summary>
        /// Unique identifier for the certification
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the certification (e.g., ISO 9001)
        /// </summary>
        public string Name { get; set; } = string.Empty;

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
        public string? AdminNotes { get; set; }
    }

    /// <summary>
    /// Data transfer object for updating certification status
    /// </summary>
    public class UpdateCertificationStatusDto
    {
        /// <summary>
        /// New status for the certification
        /// </summary>
        [Required]
        public CertificationStatus Status { get; set; }

        /// <summary>
        /// Notes from admin reviewer
        /// </summary>
        [StringLength(1000)]
        public string? AdminNotes { get; set; }
    }
}