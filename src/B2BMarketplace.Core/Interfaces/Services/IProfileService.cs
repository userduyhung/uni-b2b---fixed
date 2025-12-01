using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for profile management operations
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Gets the profile for the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userRole">User role</param>
        /// <returns>Profile data or null if not found</returns>
        Task<object?> GetProfileAsync(Guid userId, UserRole userRole);

        /// <summary>
        /// Updates the profile for the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userRole">User role</param>
        /// <param name="profileData">Profile data to update</param>
        /// <returns>Updated profile data</returns>
        Task<object> UpdateProfileAsync(Guid userId, UserRole userRole, object profileData);

        /// <summary>
        /// Gets the public profile for a verified seller
        /// </summary>
        /// <param name="sellerId">Seller ID</param>
        /// <returns>Public seller profile data or null if not found or not publicly visible</returns>
        Task<PublicSellerProfileDto?> GetPublicSellerProfileAsync(Guid sellerId);

        /// <summary>
        /// Gets a list of verified sellers with public profiles
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <param name="industry">Filter by industry</param>
        /// <param name="country">Filter by country</param>
        /// <returns>List of public seller profiles with pagination information</returns>
        Task<PublicSellerProfilesResult> GetPublicSellerProfilesAsync(int page, int pageSize, string? industry, string? country);
    }
}