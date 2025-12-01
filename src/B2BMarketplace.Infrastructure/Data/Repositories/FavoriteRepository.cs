using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for favorite data access operations
    /// </summary>
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for FavoriteRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public FavoriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a seller to a buyer's favorites
        /// </summary>
        /// <param name="favorite">Favorite entity to create</param>
        /// <returns>Created Favorite entity</returns>
        public async Task<Favorite> AddFavoriteAsync(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return favorite;
        }

        /// <summary>
        /// Removes a seller from a buyer's favorites
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>True if removed, false if not found</returns>
        public async Task<bool> RemoveFavoriteAsync(Guid buyerProfileId, Guid sellerProfileId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.BuyerProfileId == buyerProfileId && f.SellerProfileId == sellerProfileId);

            if (favorite == null)
                return false;

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Checks if a seller is favorited by a buyer
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>True if favorited, false otherwise</returns>
        public async Task<bool> IsFavoritedAsync(Guid buyerProfileId, Guid sellerProfileId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.BuyerProfileId == buyerProfileId && f.SellerProfileId == sellerProfileId);
        }

        /// <summary>
        /// Gets all favorites for a buyer
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <returns>List of favorite entities</returns>
        public async Task<List<Favorite>> GetFavoritesByBuyerAsync(Guid buyerProfileId)
        {
            return await _context.Favorites
                .Include(f => f.SellerProfile)
                .ThenInclude(sp => sp.User)
                .Where(f => f.BuyerProfileId == buyerProfileId)
                .ToListAsync();
        }

        /// <summary>
        /// Removes all favorites for a seller (when seller is deleted)
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Number of favorites removed</returns>
        public async Task<int> RemoveAllFavoritesForSellerAsync(Guid sellerProfileId)
        {
            var favorites = await _context.Favorites
                .Where(f => f.SellerProfileId == sellerProfileId)
                .ToListAsync();

            _context.Favorites.RemoveRange(favorites);
            await _context.SaveChangesAsync();

            return favorites.Count;
        }
    }
}