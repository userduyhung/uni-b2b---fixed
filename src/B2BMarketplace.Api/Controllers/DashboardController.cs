using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.Interfaces.Services;
using System;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for dashboard and statistics operations
    /// </summary>
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    [Produces("application/json")]
    public class DashboardController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IAnalyticsService _analyticsService;

        /// <summary>
        /// Constructor for DashboardController
        /// </summary>
        /// <param name="orderService">Order service</param>
        /// <param name="analyticsService">Analytics service</param>
        public DashboardController(IOrderService orderService, IAnalyticsService analyticsService)
        {
            _orderService = orderService;
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// Gets sales statistics for the current seller
        /// </summary>
        /// <param name="startDate">Start date for statistics</param>
        /// <param name="endDate">End date for statistics</param>
        /// <returns>Sales statistics data</returns>
        [HttpGet("sales-stats")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetSalesStatistics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // Get seller ID from JWT token
                var userIdClaim = User.FindFirst("nameid") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var sellerId))
                {
                    return Unauthorized(new
                    {
                        error = "Invalid user token",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Set default date range if not provided
                startDate = startDate ?? DateTime.UtcNow.AddYears(-1);
                endDate = endDate ?? DateTime.UtcNow;

                var stats = await _analyticsService.GetSellerSalesStatisticsAsync(sellerId, startDate.Value, endDate.Value);

                return Ok(new
                {
                    success = true,
                    message = "Sales statistics retrieved successfully",
                    data = new
                    {
                        itemsSold = stats.TotalProductsSold, // Use itemsSold as expected by test
                        totalRevenue = stats.TotalRevenue,
                        averageOrderValue = stats.AverageOrderValue,
                        totalOrders = stats.TotalOrders,
                        completedOrders = stats.CompletedOrders,
                        conversionRate = stats.ConversionRate,
                        topSellingProducts = stats.TopSellingProducts,
                        salesOverTime = stats.SalesOverTime,
                        startDate = stats.StartDate,
                        endDate = stats.EndDate
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving sales statistics",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }
    }
}