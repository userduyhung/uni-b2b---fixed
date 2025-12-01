using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a premium subscription for a seller
    /// </summary>
    public class PremiumSubscription
    {
        /// <summary>
        /// Unique identifier for the subscription
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the seller with the subscription
        /// </summary>
        public Guid SellerProfileId { get; set; }

        /// <summary>
        /// Seller ID (alias for SellerProfileId)
        /// </summary>
        public Guid SellerId => SellerProfileId;

        /// <summary>
        /// Navigation property to the seller profile
        /// </summary>
        [ForeignKey("SellerProfileId")]
        public virtual SellerProfile SellerProfile { get; set; } = null!;

        /// <summary>
        /// Subscription plan type
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PlanType { get; set; } = "Premium"; // Default to Premium

        /// <summary>
        /// Start date of the subscription
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the subscription
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Whether the subscription is currently active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Status (alias for IsActive)
        /// </summary>
        public bool Status => IsActive;

        /// <summary>
        /// Whether the subscription is auto-renewing
        /// </summary>
        public bool IsAutoRenewing { get; set; } = true;

        /// <summary>
        /// Payment ID associated with this subscription
        /// </summary>
        public Guid? PaymentId { get; set; }

        /// <summary>
        /// Navigation property to the payment
        /// </summary>
        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }

        /// <summary>
        /// Date and time when the subscription was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Created date (alias for CreatedAt)
        /// </summary>
        public DateTime CreatedDate => CreatedAt;

        /// <summary>
        /// Date and time when the subscription was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
