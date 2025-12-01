using System;
using System.Collections.Generic;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// DTO for dashboard statistics
    /// </summary>
    public class DashboardStatsDto
    {
        /// <summary>
        /// Total number of users
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// Total number of products
        /// </summary>
        public int TotalProducts { get; set; }

        /// <summary>
        /// Total number of orders
        /// </summary>
        public int TotalOrders { get; set; }

        /// <summary>
        /// Total revenue
        /// </summary>
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// DTO for user growth data
    /// </summary>
    public class UserGrowthDataDto
    {
        /// <summary>
        /// Date for this data point
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Number of new users on this date
        /// </summary>
        public int NewUsers { get; set; }
    }

    /// <summary>
    /// DTO for revenue report
    /// </summary>
    public class RevenueReportDto
    {
        /// <summary>
        /// Date for this data point
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Revenue on this date
        /// </summary>
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// DTO for top selling products
    /// </summary>
    public class TopSellingProductDto
    {
        /// <summary>
        /// Product ID
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Number of sales
        /// </summary>
        public int SalesCount { get; set; }

        /// <summary>
        /// Revenue generated from sales
        /// </summary>
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// DTO for system metrics
    /// </summary>
    public class SystemMetricsDto
    {
        /// <summary>
        /// Number of active users
        /// </summary>
        public int ActiveUsers { get; set; }

        /// <summary>
        /// Total number of requests
        /// </summary>
        public int TotalRequests { get; set; }

        /// <summary>
        /// Average response time in milliseconds
        /// </summary>
        public double AverageResponseTimeMs { get; set; }

        /// <summary>
        /// Memory usage in MB
        /// </summary>
        public double MemoryUsageMb { get; set; }

        /// <summary>
        /// CPU usage percentage
        /// </summary>
        public double CpuUsagePercent { get; set; }
    }
}