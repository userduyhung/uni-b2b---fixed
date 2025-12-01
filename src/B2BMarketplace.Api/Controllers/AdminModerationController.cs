using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for admin content moderation operations
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    [Produces("application/json")]
    public class ModerationController : ControllerBase
    {
        private readonly IContentModerationService _contentModerationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contentModerationService">Content moderation service</param>
        public ModerationController(IContentModerationService contentModerationService)
        {
            _contentModerationService = contentModerationService;
        }

        /// <summary>
        /// Get content reports with filtering and pagination
        /// </summary>
        /// <param name="status">Filter by report status</param>
        /// <param name="contentType">Filter by content type</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of content reports</returns>
        [HttpGet("reports")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<ContentReportDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetReports(
            [FromQuery] B2BMarketplace.Core.Enums.ReportStatus? status,
            [FromQuery] B2BMarketplace.Core.Enums.ContentType? contentType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    Status = status,
                    ContentType = contentType,
                    Page = page,
                    PageSize = pageSize
                };

                var reports = await _contentModerationService.GetReportsAsync(filter);

                return Ok(new
                {
                    message = "Content reports retrieved successfully",
                    data = reports,
                    filters = new { status, contentType, page, pageSize },
                    totalCount = reports.Count(),
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving content reports",
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ContentReportDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
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
        /// Resolve a content report
        /// </summary>
        /// <param name="id">Content report ID</param>
        /// <param name="request">Moderation resolution request</param>
        /// <returns>Resolved content report</returns>
        [HttpPut("reports/{id:guid}/resolve")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ContentReportDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResolveReport(Guid id, [FromBody] ModerationResolutionDto request)
        {
            try
            {
                // Get admin ID from claims
                var adminId = GetUserIdFromClaims();

                var report = await _contentModerationService.ResolveReportAsync(id, request, adminId);

                // Notify the user who reported the content
                await _contentModerationService.NotifyReporterAsync(id);

                return Ok(new
                {
                    message = "Content report resolved successfully",
                    data = report,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while resolving the content report",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Get repeat offenders (users with multiple resolved reports)
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of repeat offenders</returns>
        [HttpGet("offenders")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<RepeatOffenderDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRepeatOffenders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var offenders = await _contentModerationService.GetRepeatOffendersAsync(page, pageSize);

                return Ok(new
                {
                    message = "Repeat offenders retrieved successfully",
                    data = offenders,
                    pagination = new { page, pageSize },
                    totalCount = offenders.Count(),
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving repeat offenders",
                    timestamp = DateTime.UtcNow
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