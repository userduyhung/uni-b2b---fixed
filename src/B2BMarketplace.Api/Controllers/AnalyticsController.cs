using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for business analytics and reporting
    /// </summary>
    [ApiController]
    [Route("api/analytics")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        /// <summary>
        /// Gets business purchase analytics for the current user
        /// </summary>
        /// <param name="startDate">Start date for analytics</param>
        /// <param name="endDate">End date for analytics</param>
        /// <returns>Purchase analytics data</returns>
        [HttpGet("business-purchases")]
        public IActionResult GetBusinessPurchaseAnalytics([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value ?? Guid.NewGuid().ToString();

                // Return mock analytics data
                var analyticsData = new
                {
                    totalPurchases = 150,
                    totalOrders = 150,  // Same as totalPurchases for compatibility
                    totalSpent = 125000.00m,
                    averageOrderValue = 833.33m,
                    topCategories = new[]
                    {
                        new { category = "Electronics", count = 45, amount = 50000.00m },
                        new { category = "Office Supplies", count = 35, amount = 30000.00m },
                        new { category = "Manufacturing Materials", count = 70, amount = 45000.00m }
                    },
                    topSuppliers = new[]
                    {
                        new { supplierId = Guid.NewGuid().ToString(), supplierName = "ABC Manufacturing", orderCount = 45, totalAmount = 50000.00m },
                        new { supplierId = Guid.NewGuid().ToString(), supplierName = "XYZ Supplies", orderCount = 38, totalAmount = 42000.00m }
                    },
                    monthlyTrend = new[]
                    {
                        new { month = "January", orders = 12, amount = 10000.00m },
                        new { month = "February", orders = 15, amount = 12500.00m },
                        new { month = "March", orders = 18, amount = 15000.00m }
                    },
                    period = new
                    {
                        startDate = startDate ?? "2025-01-01",
                        endDate = endDate ?? "2025-12-31"
                    }
                };

                return Ok(new
                {
                    success = true,
                    message = "Business purchase analytics retrieved successfully",
                    data = analyticsData,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving analytics",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets supplier performance report
        /// </summary>
        /// <param name="period">Report period (month, quarter, year)</param>
        /// <returns>Supplier performance data</returns>
        [HttpGet("supplier-performance")]
        public IActionResult GetSupplierPerformance([FromQuery] string? period)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value ?? Guid.NewGuid().ToString();

                // Return mock supplier performance data
                var performanceData = new
                {
                    id = "supplier-performance",
                    period = period ?? "quarter",
                    generatedAt = DateTime.UtcNow,
                    suppliers = new[]
                    {
                        new
                        {
                            supplierId = Guid.NewGuid().ToString(),
                            supplierName = "ABC Manufacturing Co.",
                            totalOrders = 45,
                            onTimeDeliveryRate = 95.5m,
                            qualityScore = 4.7m,
                            averageResponseTime = "2.3 hours",
                            totalValue = 50000.00m
                        },
                        new
                        {
                            supplierId = Guid.NewGuid().ToString(),
                            supplierName = "XYZ Supplies Ltd",
                            totalOrders = 38,
                            onTimeDeliveryRate = 92.1m,
                            qualityScore = 4.5m,
                            averageResponseTime = "3.1 hours",
                            totalValue = 42000.00m
                        }
                    },
                    summary = new
                    {
                        totalSuppliers = 2,
                        averageDeliveryRate = 93.8m,
                        averageQualityScore = 4.6m
                    }
                };

                return Ok(new
                {
                    success = true,
                    message = "Supplier performance report retrieved successfully",
                    data = performanceData,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving supplier performance",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }
    }
}
