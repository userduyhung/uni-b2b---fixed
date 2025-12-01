using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface for review repository operations
    /// </summary>
    public interface IReviewRepository
    {
        /// <summary>
        /// Adds a new review
        /// </summary>
        /// <param name="review">Review to add</param>
        /// <returns>Added review</returns>
        Task<Review> AddReviewAsync(Review review);

        /// <summary>
        /// Gets reviews for a specific seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of reviews</returns>
        Task<IEnumerable<Review>> GetReviewsForSellerAsync(Guid sellerProfileId, int page, int pageSize);

        /// <summary>
        /// Gets a review by ID
        /// </summary>
        /// <param name="reviewId">Review ID</param>
        /// <returns>Review if found, null otherwise</returns>
        Task<Review?> GetReviewByIdAsync(Guid reviewId);

        /// <summary>
        /// Checks if a buyer has already reviewed a seller
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>True if buyer has already reviewed the seller, false otherwise</returns>
        Task<bool> HasBuyerReviewedSellerAsync(Guid buyerProfileId, Guid sellerProfileId);

        /// <summary>
        /// Gets rating summary for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Rating summary</returns>
        Task<SellerRatingSummaryDto> GetSellerRatingSummaryAsync(Guid sellerProfileId);

        /// <summary>
        /// Updates a review
        /// </summary>
        /// <param name="review">Review to update</param>
        /// <returns>Updated review</returns>
        Task<Review> UpdateReviewAsync(Review review);

        /// <summary>
        /// Checks if a buyer has already reviewed a product
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="productId">Product ID</param>
        /// <returns>True if buyer has already reviewed the product, false otherwise</returns>
        Task<bool> HasBuyerReviewedProductAsync(Guid buyerProfileId, Guid productId);
    }
}