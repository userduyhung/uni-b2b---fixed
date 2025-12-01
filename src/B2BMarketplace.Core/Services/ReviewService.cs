using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for review operations
    /// </summary>
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly INotificationService _notificationService;
        private readonly ISellerProfileRepository _sellerProfileRepository;

        /// <summary>
        /// Constructor for ReviewService
        /// </summary>
        /// <param name="reviewRepository">Review repository</param>
        /// <param name="notificationService">Notification service</param>
        /// <param name="sellerProfileRepository">Seller profile repository</param>
        public ReviewService(
            IReviewRepository reviewRepository, 
            INotificationService notificationService, 
            ISellerProfileRepository sellerProfileRepository)
        {
            _reviewRepository = reviewRepository;
            _notificationService = notificationService;
            _sellerProfileRepository = sellerProfileRepository;
        }

        /// <summary>
        /// Creates a new review
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="createReviewDto">Review data</param>
        /// <returns>Created review</returns>
        public async Task<ReviewDto> CreateReviewAsync(Guid buyerProfileId, CreateReviewDto createReviewDto)
        {

            // Check if buyer has already reviewed this seller
            var hasReviewed = await _reviewRepository.HasBuyerReviewedSellerAsync(buyerProfileId, createReviewDto.SellerProfileId);
            if (hasReviewed)
            {
                throw new InvalidOperationException("Buyer has already reviewed this seller");
            }

            // Create the review
            var review = new Review
            {
                Id = Guid.NewGuid(),
                BuyerProfileId = buyerProfileId,
                SellerProfileId = createReviewDto.SellerProfileId,
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                IsApproved = true
            };

            var addedReview = await _reviewRepository.AddReviewAsync(review);

            // Return the review DTO without accessing navigation properties
            return new ReviewDto
            {
                Id = addedReview.Id,
                BuyerProfileId = addedReview.BuyerProfileId,
                BuyerName = addedReview.BuyerProfile?.User?.Email ?? "Anonymous", 
                SellerProfileId = addedReview.SellerProfileId,
                Rating = addedReview.Rating,
                Comment = addedReview.Comment,
                CreatedAt = addedReview.CreatedAt,
                IsReported = addedReview.IsReported
            };
        }

        /// <summary>
        /// Gets reviews for a specific seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of reviews</returns>
        public async Task<IEnumerable<ReviewDto>> GetReviewsForSellerAsync(Guid sellerProfileId, int page, int pageSize)
        {
            var reviews = await _reviewRepository.GetReviewsForSellerAsync(sellerProfileId, page, pageSize);

            return reviews.Select(review => new ReviewDto
            {
                Id = review.Id,
                BuyerProfileId = review.BuyerProfileId,
                BuyerName = review.BuyerProfile?.User?.Email ?? "Anonymous",
                SellerProfileId = review.SellerProfileId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                IsReported = review.IsReported
            });
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
        /// Reports an inappropriate review
        /// </summary>
        /// <param name="reviewId">Review ID</param>
        /// <param name="reporterId">ID of the user reporting the review</param>
        /// <param name="reportReviewDto">Report data</param>
        /// <returns>True if reported successfully, false otherwise</returns>
        public async Task<bool> ReportReviewAsync(Guid reviewId, Guid reporterId, ReportReviewDto reportReviewDto)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
            {
                return false;
            }

            // Mark the review as reported
            review.IsReported = true;
            review.ReportedReason = reportReviewDto.Reason;
            await _reviewRepository.UpdateReviewAsync(review);

            // Create a review report record
            var reviewReport = new ReviewReport
            {
                Id = Guid.NewGuid(),
                ReviewId = reviewId,
                ReporterId = reporterId,
                Reason = reportReviewDto.Reason,
                CreatedAt = DateTime.UtcNow
            };

            // In a real implementation, we would save this to a ReviewReportRepository
            // For now, we'll just log that the report was created
            Console.WriteLine($"Review {reviewId} reported by user {reporterId} for reason: {reportReviewDto.Reason}");

            return true;
        }

        /// <summary>
        /// Checks if a buyer has already reviewed a seller
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>True if buyer has already reviewed the seller, false otherwise</returns>
        public async Task<bool> HasBuyerReviewedSellerAsync(Guid buyerProfileId, Guid sellerProfileId)
        {
            return await _reviewRepository.HasBuyerReviewedSellerAsync(buyerProfileId, sellerProfileId);
        }

        /// <summary>
        /// Creates a product review
        /// </summary>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="productId">Product ID</param>
        /// <param name="createReviewDto">Review data</param>
        /// <returns>Created review</returns>
        public async Task<ReviewDto> CreateProductReviewAsync(Guid buyerProfileId, Guid productId, CreateReviewDto createReviewDto)
        {
            // Check if buyer has already reviewed this product
            var hasReviewed = await _reviewRepository.HasBuyerReviewedProductAsync(buyerProfileId, productId);
            if (hasReviewed)
            {
                throw new InvalidOperationException("Buyer has already reviewed this product");
            }

            // Create the product review
            var review = new Review
            {
                Id = Guid.NewGuid(),
                BuyerProfileId = buyerProfileId,
                ProductId = productId, // Product-specific review
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                IsApproved = true
            };

            var addedReview = await _reviewRepository.AddReviewAsync(review);

            // For now, skip notification to avoid navigation property issues
            // In a production implementation, we'd properly handle notification of the product owner

            // Return review DTO without accessing navigation properties that might be null
            return new ReviewDto
            {
                Id = addedReview.Id,
                BuyerProfileId = addedReview.BuyerProfileId,
                BuyerName = "Anonymous",
                ProductId = addedReview.ProductId,
                Rating = addedReview.Rating,
                Comment = addedReview.Comment,
                CreatedAt = addedReview.CreatedAt,
                IsReported = addedReview.IsReported
            };
        }

        /// <summary>
        /// Updates an existing review
        /// </summary>
        /// <param name="reviewId">Review ID</param>
        /// <param name="buyerProfileId">Buyer profile ID</param>
        /// <param name="updateReviewDto">Updated review data</param>
        /// <returns>Updated review</returns>
        public async Task<ReviewDto> UpdateReviewAsync(Guid reviewId, Guid buyerProfileId, UpdateReviewDto updateReviewDto)
        {
            // Get the existing review
            var existingReview = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (existingReview == null)
            {
                throw new ArgumentException("Review not found", nameof(reviewId));
            }

            // Check if the buyer owns this review
            if (existingReview.BuyerProfileId != buyerProfileId)
            {
                throw new UnauthorizedAccessException("You can only update your own reviews");
            }

            // Update the review properties if provided
            if (updateReviewDto.Rating.HasValue)
            {
                if (updateReviewDto.Rating < 1 || updateReviewDto.Rating > 5)
                {
                    throw new ArgumentException("Rating must be between 1 and 5", nameof(updateReviewDto.Rating));
                }
                existingReview.Rating = updateReviewDto.Rating.Value;
            }

            if (!string.IsNullOrEmpty(updateReviewDto.Comment))
            {
                existingReview.Comment = updateReviewDto.Comment;
            }

            existingReview.CreatedAt = DateTime.UtcNow; // Update the timestamp

            // Save the updated review to the database
            var updatedReview = await _reviewRepository.UpdateReviewAsync(existingReview);

            // Convert to DTO and return
            return new ReviewDto
            {
                Id = updatedReview.Id,
                BuyerProfileId = updatedReview.BuyerProfileId,
                BuyerName = updatedReview.BuyerProfile?.User?.Email ?? "Anonymous",
                SellerProfileId = updatedReview.SellerProfileId,
                ProductId = updatedReview.ProductId,
                Rating = updatedReview.Rating,
                Comment = updatedReview.Comment,
                CreatedAt = updatedReview.CreatedAt,
                IsReported = updatedReview.IsReported
            };
        }
    }
}