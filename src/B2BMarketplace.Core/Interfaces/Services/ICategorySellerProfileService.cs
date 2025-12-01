using B2BMarketplace.Core.DTOs;
using System;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for managing the integration between categories and seller profiles
    /// </summary>
    public interface ICategorySellerProfileService
    {
        /// <summary>
        /// Updates seller profile with extended fields including business name and primary category
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="businessName">Business name</param>
        /// <param name="primaryCategoryId">Primary category ID</param>
        /// <returns>Updated seller profile</returns>
        Task<SellerProfileWithCertificationsDto> UpdateSellerProfileExtendedAsync(Guid sellerProfileId, string? businessName, Guid? primaryCategoryId);

        /// <summary>
        /// Updates the verified badge status for a seller based on category configuration
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="categoryId">Category ID to check requirements for</param>
        /// <returns>True if badge status was updated successfully</returns>
        Task<bool> UpdateVerifiedBadgeStatusAsync(Guid sellerProfileId, Guid categoryId);

        /// <summary>
        /// Gets seller profile with category-specific information
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Seller profile with category information</returns>
        Task<SellerProfileWithCertificationsDto> GetSellerProfileWithCategoryInfoAsync(Guid sellerProfileId);
    }
}