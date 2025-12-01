using System;
using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.Entities
{
    public class CategoryConfiguration
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public virtual ProductCategory? Category { get; set; }

        /// <summary>
        /// JSON string for required certifications for this category
        /// </summary>
        public string? RequiredCertifications { get; set; }

        /// <summary>
        /// JSON string for additional fields required for this category
        /// </summary>
        public string? AdditionalFields { get; set; }

        /// <summary>
        /// Whether sellers in this category can receive a verified badge
        /// </summary>
        public bool AllowsVerifiedBadge { get; set; }

        /// <summary>
        /// Minimum number of certifications required to get verified badge
        /// </summary>
        public int MinCertificationsForBadge { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}