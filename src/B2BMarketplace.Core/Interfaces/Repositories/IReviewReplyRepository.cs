using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for ReviewReply entities
    /// </summary>
    public interface IReviewReplyRepository
    {
        /// <summary>
        /// Creates a new review reply in the database
        /// </summary>
        /// <param name="reviewReply">Review reply entity to create</param>
        /// <returns>Created review reply entity</returns>
        Task<ReviewReply> CreateAsync(ReviewReply reviewReply);

        /// <summary>
        /// Updates an existing review reply in the database
        /// </summary>
        /// <param name="reviewReply">Review reply entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateAsync(ReviewReply reviewReply);

        /// <summary>
        /// Deletes a review reply by ID
        /// </summary>
        /// <param name="id">Review reply ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Gets a review reply by its ID
        /// </summary>
        /// <param name="id">Review reply ID to search for</param>
        /// <returns>Review reply entity if found, null otherwise</returns>
        Task<ReviewReply?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all review replies with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result containing review replies</returns>
        Task<PagedResult<ReviewReply>> GetAllAsync(int page, int pageSize);

        /// <summary>
        /// Gets a reply for a specific review
        /// </summary>
        /// <param name="reviewId">The review ID</param>
        /// <returns>The review reply or null</returns>
        Task<ReviewReply?> GetByReviewIdAsync(Guid reviewId);

        /// <summary>
        /// Gets replies by seller profile ID
        /// </summary>
        /// <param name="sellerProfileId">The seller profile ID</param>
        /// <returns>Collection of review replies</returns>
        Task<IEnumerable<ReviewReply>> GetBySellerProfileIdAsync(Guid sellerProfileId);
    }
}