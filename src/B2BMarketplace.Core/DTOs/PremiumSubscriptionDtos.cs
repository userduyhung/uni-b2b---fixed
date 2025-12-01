using System;

namespace B2BMarketplace.Core.DTOs
{
    public class PremiumSubscriptionDto
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public Guid PaymentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string PlanType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsAutoRenewing { get; set; }
    }

    public class CreatePremiumSubscriptionDto
    {
        public Guid SellerId { get; set; }
        public Guid PaymentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SellerPremiumStatusDto
    {
        public bool IsPremium { get; set; }
        public DateTime? PremiumSince { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public Guid? SubscriptionId { get; set; }
        public bool HasVerifiedBadge { get; set; }
    }
}