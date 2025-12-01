using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface for buyer profile data access operations
    /// </summary>
    public interface IBuyerProfileRepository
    {
        /// <summary>
        /// Gets a buyer profile by user ID
        /// </summary>
        /// <param name="userId">User ID to search for</param>
        /// <returns>BuyerProfile entity if found, null otherwise</returns>
        Task<BuyerProfile?> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Gets a buyer profile by ID
        /// </summary>
        /// <param name="id">Buyer profile ID to search for</param>
        /// <returns>BuyerProfile entity if found, null otherwise</returns>
        Task<BuyerProfile?> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new buyer profile
        /// </summary>
        /// <param name="profile">BuyerProfile entity to create</param>
        /// <returns>Created BuyerProfile entity</returns>
        Task<BuyerProfile> CreateAsync(BuyerProfile profile);

        /// <summary>
        /// Updates an existing buyer profile
        /// </summary>
        /// <param name="profile">BuyerProfile entity to update</param>
        /// <returns>Updated BuyerProfile entity</returns>
        Task<BuyerProfile> UpdateAsync(BuyerProfile profile);
    }
}