using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface for seller profile data access operations
    /// </summary>
    public interface ISellerProfileRepository
    {
        /// <summary>
        /// Gets a seller profile by user ID
        /// </summary>
        /// <param name="userId">User ID to search for</param>
        /// <returns>SellerProfile entity if found, null otherwise</returns>
        Task<SellerProfile?> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Gets a seller profile by ID
        /// </summary>
        /// <param name="id">ID of the seller profile</param>
        /// <returns>SellerProfile entity if found, null otherwise</returns>
        Task<SellerProfile?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets multiple seller profiles by IDs
        /// </summary>
        /// <param name="ids">List of IDs of the seller profiles</param>
        /// <returns>List of SellerProfile entities</returns>
        Task<IEnumerable<SellerProfile>> GetByIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Creates a new seller profile
        /// </summary>
        /// <param name="profile">SellerProfile entity to create</param>
        /// <returns>Created SellerProfile entity</returns>
        Task<SellerProfile> CreateAsync(SellerProfile profile);

        /// <summary>
        /// Updates an existing seller profile
        /// </summary>
        /// <param name="profile">SellerProfile entity to update</param>
        /// <returns>Updated SellerProfile entity</returns>
        Task<SellerProfile> UpdateAsync(SellerProfile profile);

        /// <summary>
        /// Gets verified seller profiles for public display with pagination and filtering
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <param name="industry">Filter by industry</param>
        /// <param name="country">Filter by country</param>
        /// <returns>Tuple containing list of seller profiles and total count</returns>
        Task<(List<SellerProfile> profiles, int totalCount)> GetVerifiedPublicProfilesAsync(int page, int pageSize, string? industry, string? country);

        /// <summary>
        /// Gets the count of premium sellers
        /// </summary>
        /// <returns>Number of premium sellers</returns>
        Task<int> GetPremiumSellerCountAsync();

        /// <summary>
        /// Gets the count of verified sellers
        /// </summary>
        /// <returns>Number of verified sellers</returns>
        Task<int> GetVerifiedSellerCountAsync();

        /// <summary>
        /// Gets all seller profiles
        /// </summary>
        /// <returns>List of all seller profiles</returns>
        Task<IEnumerable<SellerProfile>> GetAllAsync();

        /// <summary>
        /// Gets seller profiles with pagination
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <returns>Tuple containing list of seller profiles and total count</returns>
        Task<(List<SellerProfile> profiles, int totalCount)> GetAllWithPaginationAsync(int page, int pageSize);

        /// <summary>
        /// Gets rating summary for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Rating summary</returns>
        Task<SellerRatingSummaryDto> GetSellerRatingSummaryAsync(Guid sellerProfileId);

        /// <summary>
        /// Gets recent reviews for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="count">Number of recent reviews to retrieve</param>
        /// <returns>List of recent reviews</returns>
        Task<List<ReviewDto>> GetRecentReviewsAsync(Guid sellerProfileId, int count);
    }
}