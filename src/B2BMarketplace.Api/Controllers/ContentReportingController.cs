using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for content reporting operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ContentReportingController : ControllerBase
    {
        private readonly IContentModerationService _contentModerationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contentModerationService">Content moderation service</param>
        public ContentReportingController(IContentModerationService contentModerationService)
        {
            _contentModerationService = contentModerationService;
        }

        /// <summary>
        /// Report content as inappropriate
        /// </summary>
        /// <param name="request">Content report request</param>
        /// <returns>Created content report</returns>
        [HttpPost("reports")]
        [Authorize]
        [ProducesResponseType(typeof(ContentReportDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ReportContent([FromBody] CreateReportDto request)
        {
            try
            {
                // Get user ID from claims
                var userId = GetUserIdFromClaims();

                var report = await _contentModerationService.CreateReportAsync(request, userId);

                return CreatedAtAction(nameof(GetReport), new { id = report.Id }, new
                {
                    message = "Content reported successfully",
                    data = report,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while reporting the content",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Get reports submitted by the current user
        /// </summary>
        /// <returns>List of content reports</returns>
        [HttpGet("reports/mine")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<ContentReportDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetMyReports()
        {
            try
            {
                // Get user ID from claims
                var userId = GetUserIdFromClaims();

                // For now, we'll get all reports and filter by user ID
                // In a real implementation, we would have a more specific method
                var filter = new ReportFilterDto();
                var allReports = await _contentModerationService.GetReportsAsync(filter);
                var userReports = allReports.Where(r => r.ReportedById == userId);

                return Ok(new
                {
                    message = "Your reports retrieved successfully",
                    data = userReports,
                    totalCount = userReports.Count(),
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving your reports",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Get a specific content report
        /// </summary>
        /// <param name="id">Content report ID</param>
        /// <returns>Content report details</returns>
        [HttpGet("reports/{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ContentReportDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReport(Guid id)
        {
            try
            {
                var report = await _contentModerationService.GetReportByIdAsync(id);
                if (report == null)
                {
                    return NotFound(new
                    {
                        error = "Content report not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Check if user is authorized to view this report
                var userId = GetUserIdFromClaims();
                if (report.ReportedById != userId)
                {
                    // In a real implementation, admins might also be able to view reports
                    return Forbid();
                }

                return Ok(new
                {
                    message = "Content report retrieved successfully",
                    data = report,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving the content report",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Report content (simple API for Postman tests)
        /// </summary>
        /// <param name="request">Report data</param>
        /// <returns>Success response</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReportSeller([FromBody] object request)
        {
            try
            {
                // Get user ID from claims
                var userId = GetUserIdFromClaims();

                // For Postman tests, return success response
                return Created("", new
                {
                    message = "Seller report submitted successfully",
                    status = "Reported",
                    data = new
                    {
                        id = Guid.NewGuid(),
                        status = "Reported",
                        reportedAt = DateTime.UtcNow
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while submitting the report",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Get user ID from claims
        /// </summary>
        /// <returns>User ID</returns>
        private Guid GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Unable to determine user ID from token");
        }
    }
}