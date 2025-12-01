using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for updating extended seller profile information
    /// </summary>
    public class UpdateSellerProfileExtendedDto
    {
        /// <summary>
        /// Business name of the seller (may differ from company name)
        /// </summary>
        [StringLength(255)]
        public string? BusinessName { get; set; }

        /// <summary>
        /// Primary category ID for the seller's business
        /// </summary>
        public Guid? PrimaryCategoryId { get; set; }

        /// <summary>
        /// Updates the seller's verified badge status (admin only)
        /// </summary>
        public bool? HasVerifiedBadge { get; set; }
    }
}