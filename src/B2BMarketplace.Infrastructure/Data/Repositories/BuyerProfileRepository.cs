using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for buyer profile data access operations
    /// </summary>
    public class BuyerProfileRepository : IBuyerProfileRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for BuyerProfileRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public BuyerProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a buyer profile by user ID
        /// </summary>
        /// <param name="userId">User ID to search for</param>
        /// <returns>BuyerProfile entity if found, null otherwise</returns>
        public async Task<BuyerProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _context.BuyerProfiles.FirstOrDefaultAsync(bp => bp.UserId == userId);
        }

        /// <summary>
        /// Creates a new buyer profile
        /// </summary>
        /// <param name="profile">BuyerProfile entity to create</param>
        /// <returns>Created BuyerProfile entity</returns>
        public async Task<BuyerProfile> CreateAsync(BuyerProfile profile)
        {
            var entry = await _context.BuyerProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing buyer profile
        /// </summary>
        /// <param name="profile">BuyerProfile entity to update</param>
        /// <returns>Updated BuyerProfile entity</returns>
        public async Task<BuyerProfile> UpdateAsync(BuyerProfile profile)
        {
            profile.UpdatedAt = DateTime.UtcNow;
            var entry = _context.BuyerProfiles.Update(profile);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Gets a buyer profile by ID
        /// </summary>
        /// <param name="id">Buyer profile ID to search for</param>
        /// <returns>BuyerProfile entity if found, null otherwise</returns>
        public async Task<BuyerProfile?> GetByIdAsync(Guid id)
        {
            return await _context.BuyerProfiles.FirstOrDefaultAsync(bp => bp.Id == id);
        }
    }
}