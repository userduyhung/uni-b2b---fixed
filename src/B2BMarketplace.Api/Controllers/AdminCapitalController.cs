using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for administrative operations with capital A route to match Postman collection
    /// </summary>
    [ApiController]
    [Route("api/Admin")]  // Capital A route to match Postman collection expectation
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class AdminCapitalController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        /// <summary>
        /// Constructor for AdminCapitalController
        /// </summary>
        /// <param name="analyticsService">Analytics service</param>
        public AdminCapitalController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }



        /// <summary>
        /// Gets RFQ analytics (Admin only)
        /// </summary>
        /// <param name="startDate">Start date for the period</param>
        /// <param name="endDate">End date for the period</param>
        /// <param name="interval">Time interval (daily, weekly, monthly)</param>
        /// <returns>RFQ analytics data</returns>
        [HttpGet("analytics/rfqs-capital")] // Changed to avoid conflict with AdminController
        public IActionResult GetRFQAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string interval = "daily")
        {
            try
            {
                // Set default dates if not provided
                startDate = startDate ?? DateTime.UtcNow.AddMonths(-1);
                endDate = endDate ?? DateTime.UtcNow;

                // Get RFQ analytics (mock implementation)
                var rfqData = new
                {
                    timeSeries = new[]
                    {
                        new { date = startDate.Value.ToString("yyyy-MM-dd"), rfqCount = 5, totalValue = 15000m },
                        new { date = startDate.Value.AddDays(1).ToString("yyyy-MM-dd"), rfqCount = 8, totalValue = 22000m },
                        new { date = startDate.Value.AddDays(2).ToString("yyyy-MM-dd"), rfqCount = 6, totalValue = 18000m }
                    },
                    summary = new
                    {
                        totalRfqs = 19,
                        totalValue = 55000m,
                        averageValue = 2894.74m,
                        periodStart = startDate.Value,
                        periodEnd = endDate.Value
                    }
                };

                return Ok(new
                {
                    success = true,
                    message = "RFQ analytics retrieved successfully",
                    data = rfqData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving RFQ analytics",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }
    }
}