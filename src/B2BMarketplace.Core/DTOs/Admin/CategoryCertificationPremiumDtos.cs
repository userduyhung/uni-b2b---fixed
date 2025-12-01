namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// DTO for admin category operations
    /// </summary>
    public class AdminCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for admin certification operations
    /// </summary>
    public class AdminCertificationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IssuingOrganization { get; set; }
        public string? CertificateNumber { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected, Expired
        public Guid? SellerId { get; set; }
        public string? SellerName { get; set; }
        public string? DocumentPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? ApprovedById { get; set; }
        public string? RejectionReason { get; set; }
    }

    /// <summary>
    /// DTO for premium status information
    /// </summary>
    public class PremiumStatusDto
    {
        public Guid SellerId { get; set; }
        public string? SellerName { get; set; }
        public Guid ServiceTierId { get; set; }
        public string? ServiceTierName { get; set; }
        public bool HasPremium { get; set; }
        public DateTime? PremiumSince { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsAutoRenew { get; set; }
        public decimal MonthlyFee { get; set; }
        public string? Status { get; set; } // Active, Expired, Suspended
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for subscription history
    /// </summary>
    public class SubscriptionHistoryDto
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public Guid ServiceTierId { get; set; }
        public string? ServiceTierName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string Status { get; set; } = string.Empty; // Active, Expired, Cancelled
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO for premium analytics
    /// </summary>
    public class PremiumAnalyticsDto
    {
        public int TotalPremiumSellers { get; set; }
        public int NewPremiumSellers { get; set; }
        public int RenewedPremiumSellers { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AvgMonthlyFee { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}