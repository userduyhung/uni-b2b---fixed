using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for review reply data access operations
    /// </summary>
    public class ReviewReplyRepository : IReviewReplyRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for ReviewReplyRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public ReviewReplyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new review reply in the database
        /// </summary>
        /// <param name="reviewReply">Review reply entity to create</param>
        /// <returns>Created review reply entity</returns>
        public async Task<ReviewReply> CreateAsync(ReviewReply reviewReply)
        {
            var entry = await _context.ReviewReplies.AddAsync(reviewReply);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing review reply in the database
        /// </summary>
        /// <param name="reviewReply">Review reply entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(ReviewReply reviewReply)
        {
            try
            {
                _context.ReviewReplies.Update(reviewReply);
                await _context.SaveChangesAsync();
                reviewReply.UpdatedAt = DateTime.UtcNow; // Update timestamp
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a review reply by ID
        /// </summary>
        /// <param name="id">Review reply ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var reviewReply = await _context.ReviewReplies.FindAsync(id);
            if (reviewReply == null)
                return false;

            _context.ReviewReplies.Remove(reviewReply);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a review reply by its ID
        /// </summary>
        /// <param name="id">Review reply ID to search for</param>
        /// <returns>Review reply entity if found, null otherwise</returns>
        public async Task<ReviewReply?> GetByIdAsync(Guid id)
        {
            return await _context.ReviewReplies.FindAsync(id);
        }

        /// <summary>
        /// Gets all review replies with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result containing review replies</returns>
        public async Task<PagedResult<ReviewReply>> GetAllAsync(int page, int pageSize)
        {
            // Ensure page and pageSize are valid
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit max page size

            // Build query
            var query = _context.ReviewReplies.AsQueryable();

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(rr => rr.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ReviewReply>(items, totalItems, page, pageSize);
        }

        /// <summary>
        /// Gets a reply for a specific review
        /// </summary>
        /// <param name="reviewId">The review ID</param>
        /// <returns>The review reply or null</returns>
        public async Task<ReviewReply?> GetByReviewIdAsync(Guid reviewId)
        {
            return await _context.ReviewReplies
                .FirstOrDefaultAsync(rr => rr.ReviewId == reviewId);
        }

        /// <summary>
        /// Gets replies by seller profile ID
        /// </summary>
        /// <param name="sellerProfileId">The seller profile ID</param>
        /// <returns>Collection of review replies</returns>
        public async Task<IEnumerable<ReviewReply>> GetBySellerProfileIdAsync(Guid sellerProfileId)
        {
            return await _context.ReviewReplies
                .Where(rr => rr.SellerProfileId == sellerProfileId)
                .ToListAsync();
        }
    }
}