namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// DTO for platform analytics data
    /// </summary>
    public class AnalyticsDto
    {
        /// <summary>
        /// Total number of buyers registered
        /// </summary>
        public int BuyerCount { get; set; }

        /// <summary>
        /// Total number of sellers registered
        /// </summary>
        public int SellerCount { get; set; }

        /// <summary>
        /// Total number of RFQs created
        /// </summary>
        public int RFQCount { get; set; }

        /// <summary>
        /// Total number of premium sellers
        /// </summary>
        public int PremiumSellerCount { get; set; }

        /// <summary>
        /// Total number of verified sellers
        /// </summary>
        public int VerifiedSellerCount { get; set; }

        /// <summary>
        /// Total number of products listed
        /// </summary>
        public int ProductCount { get; set; }

        /// <summary>
        /// Timestamp when the analytics data was generated
        /// </summary>
        public DateTime GeneratedAt { get; set; }

        /// <summary>
        /// Additional metrics for future use
        /// </summary>
        public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
    }

    /// <summary>
    /// DTO for user growth analytics data
    /// </summary>
    public class UserGrowthAnalyticsDto
    {
        /// <summary>
        /// User growth data over time
        /// </summary>
        public List<UserGrowthDataPoint> GrowthData { get; set; } = new();

        /// <summary>
        /// Total users at start of period
        /// </summary>
        public int StartTotalUsers { get; set; }

        /// <summary>
        /// Total users at end of period
        /// </summary>
        public int EndTotalUsers { get; set; }

        /// <summary>
        /// Growth percentage over the period
        /// </summary>
        public decimal GrowthPercentage { get; set; }

        /// <summary>
        /// Period start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Period end date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Time interval used for grouping data
        /// </summary>
        public string Interval { get; set; } = "daily";
    }

    /// <summary>
    /// DTO for a single data point in user growth analytics
    /// </summary>
    public class UserGrowthDataPoint
    {
        /// <summary>
        /// Date for this data point
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Number of new buyers on this date
        /// </summary>
        public int NewBuyers { get; set; }

        /// <summary>
        /// Number of new sellers on this date
        /// </summary>
        public int NewSellers { get; set; }

        /// <summary>
        /// Total users on this date
        /// </summary>
        public int TotalUsers { get; set; }
    }

    /// <summary>
    /// DTO for RFQ analytics data
    /// </summary>
    public class RFQAnalyticsDto
    {
        /// <summary>
        /// RFQ data over time
        /// </summary>
        public List<RFQDataPoint> RFQData { get; set; } = new();

        /// <summary>
        /// Total RFQs created during the period
        /// </summary>
        public int TotalRFQs { get; set; }

        /// <summary>
        /// Total RFQs responded to during the period
        /// </summary>
        public int TotalRespondedRFQs { get; set; }

        /// <summary>
        /// Response rate percentage
        /// </summary>
        public decimal ResponseRatePercentage { get; set; }

        /// <summary>
        /// Period start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Period end date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Time interval used for grouping data
        /// </summary>
        public string Interval { get; set; } = "daily";
    }

    /// <summary>
    /// DTO for a single data point in RFQ analytics
    /// </summary>
    public class RFQDataPoint
    {
        /// <summary>
        /// Date for this data point
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Number of RFQs created on this date
        /// </summary>
        public int NewRFQs { get; set; }

        /// <summary>
        /// Number of RFQs responded to on this date
        /// </summary>
        public int RespondedRFQs { get; set; }

        /// <summary>
        /// Closed RFQs on this date
        /// </summary>
        public int ClosedRFQs { get; set; }
    }

    /// <summary>
    /// DTO for seller performance analytics data
    /// </summary>
    public class SellerPerformanceAnalyticsDto
    {
        /// <summary>
        /// Top performing sellers based on response rate
        /// </summary>
        public List<TopSeller> TopPerformers { get; set; } = new();

        /// <summary>
        /// Premium seller statistics
        /// </summary>
        public PremiumSellerStats PremiumSellerStats { get; set; } = new();

        /// <summary>
        /// Verification status statistics
        /// </summary>
        public VerificationStats VerificationStats { get; set; } = new();
    }

    /// <summary>
    /// DTO for a top performing seller
    /// </summary>
    public class TopSeller
    {
        /// <summary>
        /// Seller ID
        /// </summary>
        public Guid SellerId { get; set; }

        /// <summary>
        /// Company name
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Number of RFQs responded to
        /// </summary>
        public int RFQResponses { get; set; }

        /// <summary>
        /// Response rate percentage
        /// </summary>
        public decimal ResponseRate { get; set; }

        /// <summary>
        /// Average response time in hours
        /// </summary>
        public decimal AverageResponseTimeHours { get; set; }
    }

    /// <summary>
    /// DTO for premium seller statistics
    /// </summary>
    public class PremiumSellerStats
    {
        /// <summary>
        /// Total premium sellers
        /// </summary>
        public int TotalPremiumSellers { get; set; }

        /// <summary>
        /// Premium sellers who have responded to RFQs
        /// </summary>
        public int PremiumSellersWithRFQResponses { get; set; }

        /// <summary>
        /// Average number of RFQ responses per premium seller
        /// </summary>
        public decimal AverageRFQResponsesPerPremiumSeller { get; set; }
    }

    /// <summary>
    /// DTO for verification status statistics
    /// </summary>
    public class VerificationStats
    {
        /// <summary>
        /// Total verified sellers
        /// </summary>
        public int TotalVerifiedSellers { get; set; }

        /// <summary>
        /// Total pending verification requests
        /// </summary>
        public int TotalPendingVerifications { get; set; }

        /// <summary>
        /// Total rejected verification requests
        /// </summary>
        public int TotalRejectedVerifications { get; set; }

        /// <summary>
        /// Verification approval rate percentage
        /// </summary>
        public decimal ApprovalRatePercentage { get; set; }
    }

    /// <summary>
    /// DTO for dashboard statistics (extended version)
    /// </summary>
    public class DashboardStatsExtendedDto
    {
        /// <summary>
        /// Total number of users
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// Total number of sellers
        /// </summary>
        public int TotalSellers { get; set; }

        /// <summary>
        /// Total number of buyers
        /// </summary>
        public int TotalBuyers { get; set; }

        /// <summary>
        /// Total number of RFQs
        /// </summary>
        public int TotalRFQs { get; set; }

        /// <summary>
        /// Total number of quotes
        /// </summary>
        public int TotalQuotes { get; set; }

        /// <summary>
        /// Number of active RFQs
        /// </summary>
        public int ActiveRFQs { get; set; }

        /// <summary>
        /// Number of pending verifications
        /// </summary>
        public int PendingVerifications { get; set; }

        /// <summary>
        /// Number of recent registrations (last 30 days)
        /// </summary>
        public int RecentRegistrations { get; set; }

        /// <summary>
        /// Top categories by RFQ count
        /// </summary>
        public List<TopCategoryDto> TopCategories { get; set; } = new();
    }

    /// <summary>
    /// DTO for top category information
    /// </summary>
    public class TopCategoryDto
    {
        /// <summary>
        /// Category name
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Number of RFQs in this category
        /// </summary>
        public int RFQCount { get; set; }
    }

    /// <summary>
    /// DTO for seller sales statistics
    /// </summary>
    public class SellerSalesStatisticsDto
    {
        /// <summary>
        /// Total number of products sold
        /// </summary>
        public int TotalProductsSold { get; set; }

        /// <summary>
        /// Total number of items sold (alias for TotalProductsSold)
        /// </summary>
        public int itemsSold => TotalProductsSold;

        /// <summary>
        /// Total revenue generated
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Average order value
        /// </summary>
        public decimal AverageOrderValue { get; set; }

        /// <summary>
        /// Total number of orders received
        /// </summary>
        public int TotalOrders { get; set; }

        /// <summary>
        /// Number of completed orders
        /// </summary>
        public int CompletedOrders { get; set; }

        /// <summary>
        /// Conversion rate (percentage of RFQs that resulted in orders)
        /// </summary>
        public decimal ConversionRate { get; set; }

        /// <summary>
        /// Top selling products
        /// </summary>
        public List<TopSellingProductDto> TopSellingProducts { get; set; } = new();

        /// <summary>
        /// Sales data over time
        /// </summary>
        public List<SalesDataPoint> SalesOverTime { get; set; } = new();

        /// <summary>
        /// Period start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Period end date
        /// </summary>
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// DTO for a sales data point over time
    /// </summary>
    public class SalesDataPoint
    {
        /// <summary>
        /// Date for this data point
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Revenue for this date
        /// </summary>
        public decimal Revenue { get; set; }

        /// <summary>
        /// Number of orders for this date
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// Number of unique customers for this date
        /// </summary>
        public int CustomerCount { get; set; }
    }
}