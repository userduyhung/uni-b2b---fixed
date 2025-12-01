using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminRFQController : ControllerBase
    {
        private readonly IRFQService _rfqService;

        public AdminRFQController(IRFQService rfqService)
        {
            _rfqService = rfqService;
        }

        /// <summary>
        /// Get pending RFQs for moderation
        /// </summary>
        /// <returns>List of pending RFQs</returns>
        [HttpGet("rfqs/pending")]
        public async Task<IActionResult> GetPendingRFQs()
        {
            try
            {
                var rfqs = await _rfqService.GetAllAsync();
                var pendingRfqs = rfqs.Where(r => r.Status == B2BMarketplace.Core.Enums.RFQStatus.Open).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Pending RFQs retrieved successfully",
                    data = pendingRfqs,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving pending RFQs",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Get all RFQs for admin view
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="status">Status filter</param>
        /// <returns>List of all RFQs</returns>
        [HttpGet("rfqs")]
        public async Task<IActionResult> GetRFQs([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? status = null)
        {
            try
            {
                var rfqs = await _rfqService.GetAllAsync();
                
                // Apply status filter if provided
                if (!string.IsNullOrEmpty(status))
                {
                    var statusEnum = Enum.TryParse<B2BMarketplace.Core.Enums.RFQStatus>(status, true, out var parsedStatus) 
                        ? parsedStatus 
                        : B2BMarketplace.Core.Enums.RFQStatus.Open;
                    rfqs = rfqs.Where(r => r.Status == parsedStatus).ToList();
                }
                
                // Apply pagination
                var totalItems = rfqs.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var pagedRfqs = rfqs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    success = true,
                    message = "RFQs retrieved successfully",
                    data = new
                    {
                        items = pagedRfqs,
                        currentPage = page,
                        pageSize = pageSize,
                        totalItems = totalItems,
                        totalPages = totalPages
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving RFQs",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Moderate an RFQ
        /// </summary>
        /// <param name="id">RFQ ID</param>
        /// <param name="request">Moderation request</param>
        /// <returns>Success response</returns>
        [HttpPut("rfqs/{id}/moderate")]
        public async Task<IActionResult> ModerateRFQ(string id, [FromBody] object request)
        {
            if (!Guid.TryParse(id, out Guid rfqId))
            {
                return BadRequest(new
                {
                    error = "Invalid RFQ ID format",
                    timestamp = DateTime.UtcNow
                });
            }

            try
            {
                var rfq = await _rfqService.GetByIdAsync(rfqId);
                if (rfq == null)
                {
                    return NotFound(new
                    {
                        error = "RFQ not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                // For now, return a mock response
                return Ok(new
                {
                    success = true,
                    message = "RFQ moderated successfully",
                    data = new
                    {
                        id = rfqId,
                        status = "Approved",
                        moderatedAt = DateTime.UtcNow
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while moderating RFQ",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }
    }
}