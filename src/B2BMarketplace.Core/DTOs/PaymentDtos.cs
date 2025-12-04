using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// DTO for initiating a payment
    /// </summary>
    public class InitiatePaymentDto
    {
        /// <summary>
        /// Amount to charge
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code (e.g., USD, EUR)
        /// </summary>
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter code")]
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// Description of the payment
        /// </summary>
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Payment method (e.g., Credit Card, Bank Transfer)
        /// </summary>
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>
        /// Token or ID from the payment provider
        /// </summary>
        [Required]
        public string PaymentToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for payment confirmation
    /// </summary>
    public class PaymentConfirmationDto
    {
        /// <summary>
        /// Payment ID
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Payment provider transaction ID
        /// </summary>
        public string? ProviderTransactionId { get; set; }

        /// <summary>
        /// Whether the payment was successful
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Error message if payment failed
        /// </summary>
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// DTO for processing a refund
    /// </summary>
    public class RefundPaymentDto
    {
        /// <summary>
        /// Payment ID to refund
        /// </summary>
        [Required]
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Reason for the refund
        /// </summary>
        [Required]
        [MaxLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Amount to refund (optional, defaults to full amount)
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal? Amount { get; set; }
    }

    /// <summary>
    /// DTO representing a payment
    /// </summary>
    public class PaymentDto
    {
        /// <summary>
        /// Unique identifier for the payment
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Payment amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code
        /// </summary>
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// Payment provider used
        /// </summary>
        public string PaymentProvider { get; set; } = string.Empty;

        /// <summary>
        /// Payment status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Payment method used
        /// </summary>
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// Description of the payment
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Date and time when the payment was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the payment was completed
        /// </summary>
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// DTO for admin-facing payment view with buyer/seller information
    /// </summary>
    public class AdminPaymentDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public string PaymentProvider { get; set; } = string.Empty;
        public string? ProviderTransactionId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Seller and buyer display names
        public string? SellerName { get; set; }
        public string? BuyerName { get; set; }

        // If available, associated order id (string)
        public string? OrderId { get; set; }
    }
}