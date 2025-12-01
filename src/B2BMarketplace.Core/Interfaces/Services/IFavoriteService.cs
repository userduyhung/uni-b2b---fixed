using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for favorite/wishlist management operations
    /// </summary>
    public interface IFavoriteService
    {
        /// <summary>
        /// Adds a seller to a buyer's favorites
        /// </summary>
        /// <param name="buyerUserId">Buyer's user ID</param>
        /// <param name="sellerProfileId">Seller's profile ID</param>
        /// <returns>True if successfully added, false if already favorited</returns>
        Task<bool> AddFavoriteAsync(Guid buyerUserId, Guid sellerProfileId);

        /// <summary>
        /// Removes a seller from a buyer's favorites
        /// </summary>
        /// <param name="buyerUserId">Buyer's user ID</param>
        /// <param name="sellerProfileId">Seller's profile ID</param>
        /// <returns>True if successfully removed, false if not found</returns>
        Task<bool> RemoveFavoriteAsync(Guid buyerUserId, Guid sellerProfileId);

        /// <summary>
        /// Checks if a seller is favorited by a buyer
        /// </summary>
        /// <param name="buyerUserId">Buyer's user ID</param>
        /// <param name="sellerProfileId">Seller's profile ID</param>
        /// <returns>True if favorited, false otherwise</returns>
        Task<bool> IsFavoritedAsync(Guid buyerUserId, Guid sellerProfileId);

        /// <summary>
        /// Gets all favorite sellers for a buyer
        /// </summary>
        /// <param name="buyerUserId">Buyer's user ID</param>
        /// <returns>List of public seller profiles</returns>
        Task<List<PublicSellerProfileDto>> GetFavoritesAsync(Guid buyerUserId);

        /// <summary>
        /// Removes a seller from all buyers' favorites (when seller is deleted)
        /// </summary>
        /// <param name="sellerProfileId">Seller's profile ID</param>
        /// <returns>Number of favorites removed</returns>
        Task<int> RemoveAllFavoritesForSellerAsync(Guid sellerProfileId);
    }
}