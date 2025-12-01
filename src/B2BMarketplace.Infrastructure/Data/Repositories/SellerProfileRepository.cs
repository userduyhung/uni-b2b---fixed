using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for seller profile data access operations
    /// </summary>
    public class SellerProfileRepository : ISellerProfileRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IReviewRepository _reviewRepository;

        /// <summary>
        /// Constructor for SellerProfileRepository
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="reviewRepository">Review repository</param>
        public SellerProfileRepository(ApplicationDbContext context, IReviewRepository reviewRepository)
        {
            _context = context;
            _reviewRepository = reviewRepository;
        }

        /// <summary>
        /// Gets a seller profile by user ID
        /// </summary>
        /// <param name="userId">User ID to search for</param>
        /// <returns>SellerProfile entity if found, null otherwise</returns>
        public async Task<SellerProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _context.SellerProfiles.FirstOrDefaultAsync(sp => sp.UserId == userId);
        }

        /// <summary>
        /// Gets a seller profile by ID
        /// </summary>
        /// <param name="id">ID of the seller profile</param>
        /// <returns>SellerProfile entity if found, null otherwise</returns>
        public async Task<SellerProfile?> GetByIdAsync(Guid id)
        {
            return await _context.SellerProfiles.FirstOrDefaultAsync(sp => sp.Id == id);
        }

        /// <summary>
        /// Gets multiple seller profiles by IDs
        /// </summary>
        /// <param name="ids">List of IDs of the seller profiles</param>
        /// <returns>List of SellerProfile entities</returns>
        public async Task<IEnumerable<SellerProfile>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.SellerProfiles.Where(sp => ids.Contains(sp.Id)).ToListAsync();
        }

        /// <summary>
        /// Creates a new seller profile
        /// </summary>
        /// <param name="profile">SellerProfile entity to create</param>
        /// <returns>Created SellerProfile entity</returns>
        public async Task<SellerProfile> CreateAsync(SellerProfile profile)
        {
            var entry = await _context.SellerProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing seller profile
        /// </summary>
        /// <param name="profile">SellerProfile entity to update</param>
        /// <returns>Updated SellerProfile entity</returns>
        public async Task<SellerProfile> UpdateAsync(SellerProfile profile)
        {
            profile.UpdatedAt = DateTime.UtcNow;
            var entry = _context.SellerProfiles.Update(profile);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Gets verified seller profiles for public display with pagination and filtering
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <param name="industry">Filter by industry</param>
        /// <param name="country">Filter by country</param>
        /// <returns>Tuple containing list of seller profiles and total count</returns>
        public async Task<(List<SellerProfile> profiles, int totalCount)> GetVerifiedPublicProfilesAsync(int page, int pageSize, string? industry, string? country)
        {
            var query = _context.SellerProfiles
                .Where(sp => sp.IsVerified) // Only verified profiles are publicly visible
                .AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(industry))
            {
                query = query.Where(sp => sp.Industry != null && sp.Industry.Contains(industry));
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(sp => sp.Country.Contains(country));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var profiles = await query
                .OrderBy(sp => sp.CompanyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (profiles, totalCount);
        }

        /// <summary>
        /// Gets the count of premium sellers
        /// </summary>
        /// <returns>Number of premium sellers</returns>
        public async Task<int> GetPremiumSellerCountAsync()
        {
            return await _context.SellerProfiles.CountAsync(sp => sp.IsPremium);
        }

        /// <summary>
        /// Gets the count of verified sellers
        /// </summary>
        /// <returns>Number of verified sellers</returns>
        public async Task<int> GetVerifiedSellerCountAsync()
        {
            return await _context.SellerProfiles.CountAsync(sp => sp.IsVerified);
        }

        /// <summary>
        /// Gets all seller profiles
        /// </summary>
        /// <returns>List of all seller profiles</returns>
        public async Task<IEnumerable<SellerProfile>> GetAllAsync()
        {
            return await _context.SellerProfiles.ToListAsync();
        }

        /// <summary>
        /// Gets seller profiles with pagination
        /// </summary>
        /// <param name=\"page\">Page number for pagination</param>
        /// <param name=\"pageSize\">Number of results per page</param>
        /// <returns>Tuple containing list of seller profiles and total count</returns>
        public async Task<(List<SellerProfile> profiles, int totalCount)> GetAllWithPaginationAsync(int page, int pageSize)
        {
            // Get total count
            var totalCount = await _context.SellerProfiles.CountAsync();

            // Apply pagination
            var profiles = await _context.SellerProfiles
                .OrderBy(sp => sp.CompanyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (profiles, totalCount);
        }

        /// <summary>
        /// Gets rating summary for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Rating summary</returns>
        public async Task<SellerRatingSummaryDto> GetSellerRatingSummaryAsync(Guid sellerProfileId)
        {
            return await _reviewRepository.GetSellerRatingSummaryAsync(sellerProfileId);
        }

        /// <summary>
        /// Gets recent reviews for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="count">Number of recent reviews to retrieve</param>
        /// <returns>List of recent reviews</returns>
        public async Task<List<ReviewDto>> GetRecentReviewsAsync(Guid sellerProfileId, int count)
        {
            var reviews = await _reviewRepository.GetReviewsForSellerAsync(sellerProfileId, 1, count);
            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                BuyerProfileId = r.BuyerProfileId,
                BuyerName = r.BuyerProfile?.Name ?? "Anonymous",
                SellerProfileId = r.SellerProfileId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                IsReported = r.IsReported
            }).ToList();
        }
    }
}