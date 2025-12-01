using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for favorite/wishlist management operations
    /// </summary>
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IBuyerProfileRepository _buyerProfileRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly IProfileService _profileService;

        /// <summary>
        /// Constructor for FavoriteService
        /// </summary>
        /// <param name="favoriteRepository">Favorite repository</param>
        /// <param name="buyerProfileRepository">Buyer profile repository</param>
        /// <param name="sellerProfileRepository">Seller profile repository</param>
        /// <param name="profileService">Profile service</param>
        public FavoriteService(
            IFavoriteRepository favoriteRepository,
            IBuyerProfileRepository buyerProfileRepository,
            ISellerProfileRepository sellerProfileRepository,
            IProfileService profileService)
        {
            _favoriteRepository = favoriteRepository;
            _buyerProfileRepository = buyerProfileRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _profileService = profileService;
        }

        /// <summary>
        /// Adds a seller to a buyer's favorites
        /// </summary>
        /// <param name="buyerUserId">Buyer's user ID</param>
        /// <param name="sellerProfileId">Seller's profile ID</param>
        /// <returns>True if successfully added, false if already favorited</returns>
        public async Task<bool> AddFavoriteAsync(Guid buyerUserId, Guid sellerProfileId)
        {
            // Validate that buyer and seller exist
            var buyerProfile = await _buyerProfileRepository.GetByUserIdAsync(buyerUserId);
            if (buyerProfile == null)
            {
                throw new ArgumentException("Buyer profile not found", nameof(buyerUserId));
            }

            var sellerProfile = await _sellerProfileRepository.GetByIdAsync(sellerProfileId);
            if (sellerProfile == null)
            {
                throw new ArgumentException("Seller profile not found", nameof(sellerProfileId));
            }

            // Check if already favorited
            if (await _favoriteRepository.IsFavoritedAsync(buyerProfile.Id, sellerProfileId))
            {
                return false; // Already favorited
            }

            // Create new favorite
            var favorite = new Favorite
            {
                BuyerProfileId = buyerProfile.Id,
                SellerProfileId = sellerProfileId
            };

            await _favoriteRepository.AddFavoriteAsync(favorite);
            return true;
        }

        /// <summary>
        /// Removes a seller from a buyer's favorites
        /// </summary>
        /// <param name="buyerUserId">Buyer's user ID</param>
        /// <param name="sellerProfileId">Seller's profile ID</param>
        /// <returns>True if successfully removed, false if not found</returns>
        public async Task<bool> RemoveFavoriteAsync(Guid buyerUserId, Guid sellerProfileId)
        {
            var buyerProfile = await _buyerProfileRepository.GetByUserIdAsync(buyerUserId);
            if (buyerProfile == null)
            {
                throw new ArgumentException("Buyer profile not found", nameof(buyerUserId));
            }

            return await _favoriteRepository.RemoveFavoriteAsync(buyerProfile.Id, sellerProfileId);
        }

        /// <summary>
        /// Checks if a seller is favorited by a buyer
        /// </summary>
        /// <param name="buyerUserId">Buyer's user ID</param>
        /// <param name="sellerProfileId">Seller's profile ID</param>
        /// <returns>True if favorited, false otherwise</returns>
        public async Task<bool> IsFavoritedAsync(Guid buyerUserId, Guid sellerProfileId)
        {
            var buyerProfile = await _buyerProfileRepository.GetByUserIdAsync(buyerUserId);
            if (buyerProfile == null)
            {
                throw new ArgumentException("Buyer profile not found", nameof(buyerUserId));
            }

            return await _favoriteRepository.IsFavoritedAsync(buyerProfile.Id, sellerProfileId);
        }

        /// <summary>
        /// Gets all favorite sellers for a buyer
        /// </summary>
        /// <param name="buyerUserId">Buyer's user ID</param>
        /// <returns>List of public seller profiles</returns>
        public async Task<List<PublicSellerProfileDto>> GetFavoritesAsync(Guid buyerUserId)
        {
            var buyerProfile = await _buyerProfileRepository.GetByUserIdAsync(buyerUserId);
            if (buyerProfile == null)
            {
                // Return empty list if buyer profile doesn't exist yet
                return new List<PublicSellerProfileDto>();
            }

            var favorites = await _favoriteRepository.GetFavoritesByBuyerAsync(buyerProfile.Id);

            var result = new List<PublicSellerProfileDto>();
            foreach (var favorite in favorites)
            {
                // Get seller profile as public profile
                var publicProfile = await _profileService.GetPublicSellerProfileAsync(favorite.SellerProfileId);
                if (publicProfile != null)
                {
                    result.Add(publicProfile);
                }
            }

            return result;
        }

        /// <summary>
        /// Removes a seller from all buyers' favorites (when seller is deleted)
        /// </summary>
        /// <param name="sellerProfileId">Seller's profile ID</param>
        /// <returns>Number of favorites removed</returns>
        public async Task<int> RemoveAllFavoritesForSellerAsync(Guid sellerProfileId)
        {
            return await _favoriteRepository.RemoveAllFavoritesForSellerAsync(sellerProfileId);
        }
    }
}