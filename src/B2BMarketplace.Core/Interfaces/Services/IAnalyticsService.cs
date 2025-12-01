using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for analytics and reporting operations
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Gets platform analytics data including user counts, RFQ statistics, and premium seller counts
        /// </summary>
        /// <returns>Analytics data with key metrics</returns>
        Task<AnalyticsDto> GetPlatformAnalyticsAsync();

        /// <summary>
        /// Gets user statistics over time
        /// </summary>
        /// <param name="startDate">Start date for the period</param>
        /// <param name="endDate">End date for the period</param>
        /// <param name="interval">Time interval for grouping (daily, weekly, monthly)</param>
        /// <returns>User growth analytics data</returns>
        Task<UserGrowthAnalyticsDto> GetUserGrowthAnalyticsAsync(DateTime startDate, DateTime endDate, string interval = "daily");

        /// <summary>
        /// Gets RFQ statistics over time
        /// </summary>
        /// <param name="startDate">Start date for the period</param>
        /// <param name="endDate">End date for the period</param>
        /// <param name="interval">Time interval for grouping (daily, weekly, monthly)</param>
        /// <returns>RFQ analytics data</returns>
        Task<RFQAnalyticsDto> GetRFQAnalyticsAsync(DateTime startDate, DateTime endDate, string interval = "daily");

        /// <summary>
        /// Gets seller performance statistics
        /// </summary>
        /// <returns>Seller performance analytics data</returns>
        Task<SellerPerformanceAnalyticsDto> GetSellerPerformanceAnalyticsAsync();

        /// <summary>
        /// Gets dashboard statistics
        /// </summary>
        /// <returns>Dashboard statistics</returns>
        Task<DashboardStatsDto> GetDashboardStatsAsync();

        /// <summary>
        /// Gets dashboard statistics (extended version)
        /// </summary>
        /// <returns>Extended dashboard statistics</returns>
        Task<DashboardStatsExtendedDto> GetDashboardStatsExtendedAsync();

        /// <summary>
        /// Gets user growth data
        /// </summary>
        /// <param name="startDate">Start date for the period</param>
        /// <param name="endDate">End date for the period</param>
        /// <returns>User growth data</returns>
        Task<UserGrowthDataDto[]> GetUserGrowthDataAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets revenue report
        /// </summary>
        /// <param name="startDate">Start date for the period</param>
        /// <param name="endDate">End date for the period</param>
        /// <returns>Revenue report</returns>
        Task<RevenueReportDto[]> GetRevenueReportAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets top selling products
        /// </summary>
        /// <param name="limit">Maximum number of products to return</param>
        /// <returns>Top selling products</returns>
        Task<TopSellingProductDto[]> GetTopSellingProductsAsync(int limit);

        /// <summary>
        /// Gets system metrics
        /// </summary>
        /// <returns>System metrics</returns>
        Task<SystemMetricsDto> GetSystemMetricsAsync();

        /// <summary>
        /// Gets sales statistics for a specific seller
        /// </summary>
        /// <param name="sellerId">Seller ID</param>
        /// <param name="startDate">Start date for statistics</param>
        /// <param name="endDate">End date for statistics</param>
        /// <returns>Sales statistics for the seller</returns>
        Task<SellerSalesStatisticsDto> GetSellerSalesStatisticsAsync(Guid sellerId, DateTime startDate, DateTime endDate);
    }
}