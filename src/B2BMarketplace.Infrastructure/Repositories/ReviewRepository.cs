using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for review operations
    /// </summary>
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for ReviewRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new review
        /// </summary>
        /// <param name="review">Review to add</param>
        /// <returns>Added review</returns>
        public async Task<Review> AddReviewAsync(Review review)
        {
            _ = _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        /// <summary>
        /// Gets reviews for a specific seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of reviews</returns>
        public async Task<IEnumerable<Review>> GetReviewsForSellerAsync(Guid sellerProfileId, int page, int pageSize)
        {
            return await _context.Reviews
                .Include(r => r.BuyerProfile)
                    .ThenInclude(bp => bp.User)
                .Where(r => r.SellerProfileId == sellerProfileId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a review by ID
        /// </summary>
        /// <param name="reviewId">Review ID</param>
        /// <returns>Review if found, null otherwise</returns>
        public async Task<Review?> GetReviewByIdAsync(Guid reviewId)
        {
            return await _context.Reviews
                .Include(r => r.BuyerProfile)
                    .ThenInclude(bp => bp.User)
                .Include(r => r.SellerProfile)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        /// <summary>
        /// Checks if a buyer has already reviewed a seller
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>True if buyer has already reviewed the seller, false otherwise</returns>
        public async Task<bool> HasBuyerReviewedSellerAsync(Guid buyerProfileId, Guid sellerProfileId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.BuyerProfileId == buyerProfileId
                    && r.SellerProfileId == sellerProfileId);
        }

        /// <summary>
        /// Gets rating summary for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Rating summary</returns>
        public async Task<SellerRatingSummaryDto> GetSellerRatingSummaryAsync(Guid sellerProfileId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.SellerProfileId == sellerProfileId && r.IsApproved)
                .ToListAsync();

            if (!reviews.Any())
            {
                return new SellerRatingSummaryDto
                {
                    AverageRating = 0,
                    TotalRatings = 0,
                    RatingDistribution = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } }
                };
            }

            var averageRating = reviews.Average(r => r.Rating);
            var totalRatings = reviews.Count;

            var ratingDistribution = new Dictionary<int, int>
            {
                { 1, reviews.Count(r => r.Rating == 1) },
                { 2, reviews.Count(r => r.Rating == 2) },
                { 3, reviews.Count(r => r.Rating == 3) },
                { 4, reviews.Count(r => r.Rating == 4) },
                { 5, reviews.Count(r => r.Rating == 5) }
            };

            return new SellerRatingSummaryDto
            {
                AverageRating = Math.Round(averageRating, 2),
                TotalRatings = totalRatings,
                RatingDistribution = ratingDistribution
            };
        }

        /// <summary>
        /// Updates a review
        /// </summary>
        /// <param name="review">Review to update</param>
        /// <returns>Updated review</returns>
        public async Task<Review> UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        /// <summary>
        /// Checks if a buyer has already reviewed a product
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="productId">Product ID</param>
        /// <returns>True if buyer has already reviewed the product, false otherwise</returns>
        public async Task<bool> HasBuyerReviewedProductAsync(Guid buyerProfileId, Guid productId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.BuyerProfileId == buyerProfileId
                    && r.ProductId == productId);
        }
    }
}