using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// DTO for creating a new review
    /// </summary>
    public class CreateReviewDto
    {
        /// <summary>
        /// ID of the seller being reviewed
        /// </summary>
        [Required]
        public Guid SellerProfileId { get; set; }

        /// <summary>
        /// Rating value (1-5 stars)
        /// </summary>
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        /// <summary>
        /// Review comment/text
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Review comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
    }

    /// <summary>
    /// DTO for reporting an inappropriate review
    /// </summary>
    public class ReportReviewDto
    {
        /// <summary>
        /// Reason for reporting the review
        /// </summary>
        [Required]
        [MaxLength(500, ErrorMessage = "Report reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO representing a review
    /// </summary>
    public class ReviewDto
    {
        /// <summary>
        /// Unique identifier for the review
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the buyer who submitted the review
        /// </summary>
        public Guid BuyerProfileId { get; set; }

        /// <summary>
        /// Buyer's name
        /// </summary>
        public string BuyerName { get; set; } = string.Empty;

        /// <summary>
        /// ID of the seller being reviewed
        /// </summary>
        public Guid SellerProfileId { get; set; }

        /// <summary>
        /// Rating value (1-5 stars)
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Review comment/text
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Date and time when the review was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Whether the review has been reported as inappropriate
        /// </summary>
        public bool IsReported { get; set; }

        /// <summary>
        /// ID of the product being reviewed (optional - for product reviews)
        /// </summary>
        public Guid? ProductId { get; set; }
    }

    /// <summary>
    /// DTO for seller rating summary
    /// </summary>
    public class SellerRatingSummaryDto
    {
        /// <summary>
        /// Average rating of the seller
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// Total number of ratings received
        /// </summary>
        public int TotalRatings { get; set; }

        /// <summary>
        /// Distribution of ratings (1-5 stars)
        /// </summary>
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
    }

    /// <summary>
    /// DTO for updating a review
    /// </summary>
    public class UpdateReviewDto
    {
        /// <summary>
        /// New rating value (1-5 stars)
        /// </summary>
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int? Rating { get; set; }

        /// <summary>
        /// New review comment/text
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Review comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
    }
}