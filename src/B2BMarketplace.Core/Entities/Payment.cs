using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a payment transaction
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// Unique identifier for the payment
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the seller making the payment
        /// </summary>
        public Guid SellerProfileId { get; set; }

        /// <summary>
        /// Navigation property to the seller profile
        /// </summary>
        [ForeignKey("SellerProfileId")]
        public virtual SellerProfile SellerProfile { get; set; } = null!;

        /// <summary>
        /// Payment amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code (e.g., USD, EUR)
        /// </summary>
        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// Payment provider used (e.g., Stripe, PayPal)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PaymentProvider { get; set; } = string.Empty;

        /// <summary>
        /// Payment provider transaction ID
        /// </summary>
        [StringLength(255)]
        public string? ProviderTransactionId { get; set; }

        /// <summary>
        /// Associated Order ID (string) if this payment is for an order
        /// </summary>
        [StringLength(200)]
        public string? OrderId { get; set; }

        /// <summary>
        /// Navigation property to the Order (optional)
        /// </summary>
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        public PaymentStatus Status { get; set; }

        /// <summary>
        /// Payment method used (e.g., Credit Card, Bank Transfer)
        /// </summary>
        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// Description of the payment
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Date and time when the payment was initiated
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date and time when the payment was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date and time when the payment was completed (if applicable)
        /// </summary>
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// Payment status enumeration
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Payment is pending processing
        /// </summary>
        Pending,

        /// <summary>
        /// Payment has been successfully completed
        /// </summary>
        Completed,

        /// <summary>
        /// Payment has failed
        /// </summary>
        Failed,

        /// <summary>
        /// Payment has been refunded
        /// </summary>
        Refunded,

        /// <summary>
        /// Payment is being processed
        /// </summary>
        Processing
    }
}