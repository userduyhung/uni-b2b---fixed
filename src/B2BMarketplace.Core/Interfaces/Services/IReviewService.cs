using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for review service operations
    /// </summary>
    public interface IReviewService
    {
        /// <summary>
        /// Creates a new review
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="createReviewDto">Review data</param>
        /// <returns>Created review</returns>
        Task<ReviewDto> CreateReviewAsync(Guid buyerProfileId, CreateReviewDto createReviewDto);

        /// <summary>
        /// Gets reviews for a specific seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of reviews</returns>
        Task<IEnumerable<ReviewDto>> GetReviewsForSellerAsync(Guid sellerProfileId, int page, int pageSize);

        /// <summary>
        /// Gets rating summary for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Rating summary</returns>
        Task<SellerRatingSummaryDto> GetSellerRatingSummaryAsync(Guid sellerProfileId);

        /// <summary>
        /// Reports an inappropriate review
        /// </summary>
        /// <param name="reviewId">Review ID</param>
        /// <param name="reporterId">ID of the user reporting the review</param>
        /// <param name="reportReviewDto">Report data</param>
        /// <returns>True if reported successfully, false otherwise</returns>
        Task<bool> ReportReviewAsync(Guid reviewId, Guid reporterId, ReportReviewDto reportReviewDto);

        /// <summary>
        /// Checks if a buyer has already reviewed a seller
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>True if buyer has already reviewed the seller, false otherwise</returns>
        Task<bool> HasBuyerReviewedSellerAsync(Guid buyerProfileId, Guid sellerProfileId);

        /// <summary>
        /// Creates a product review
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="productId">Product ID</param>
        /// <param name="createReviewDto">Review data</param>
        /// <returns>Created review</returns>
        Task<ReviewDto> CreateProductReviewAsync(Guid buyerProfileId, Guid productId, CreateReviewDto createReviewDto);

        /// <summary>
        /// Updates an existing review
        /// </summary>
        /// <param name="reviewId">Review ID</param>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="updateReviewDto">Updated review data</param>
        /// <returns>Updated review</returns>
        Task<ReviewDto> UpdateReviewAsync(Guid reviewId, Guid buyerProfileId, UpdateReviewDto updateReviewDto);
    }
}