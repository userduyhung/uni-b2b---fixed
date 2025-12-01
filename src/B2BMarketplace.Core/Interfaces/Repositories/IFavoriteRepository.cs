using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface for favorite data access operations
    /// </summary>
    public interface IFavoriteRepository
    {
        /// <summary>
        /// Adds a seller to a buyer's favorites
        /// </summary>
        /// <param name="favorite">Favorite entity to create</param>
        /// <returns>Created Favorite entity</returns>
        Task<Favorite> AddFavoriteAsync(Favorite favorite);

        /// <summary>
        /// Removes a seller from a buyer's favorites
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>True if removed, false if not found</returns>
        Task<bool> RemoveFavoriteAsync(Guid buyerProfileId, Guid sellerProfileId);

        /// <summary>
        /// Checks if a seller is favorited by a buyer
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>True if favorited, false otherwise</returns>
        Task<bool> IsFavoritedAsync(Guid buyerProfileId, Guid sellerProfileId);

        /// <summary>
        /// Gets all favorites for a buyer
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <returns>List of favorite entities</returns>
        Task<List<Favorite>> GetFavoritesByBuyerAsync(Guid buyerProfileId);

        /// <summary>
        /// Removes all favorites for a seller (when seller is deleted)
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Number of favorites removed</returns>
        Task<int> RemoveAllFavoritesForSellerAsync(Guid sellerProfileId);
    }
}