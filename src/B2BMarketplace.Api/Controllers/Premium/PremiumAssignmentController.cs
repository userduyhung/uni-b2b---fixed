using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services.Premium;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BMarketplace.Api.Controllers.Premium
{
    /// <summary>
    /// Controller for premium status assignment operations
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin/premium")]
    public class PremiumAssignmentController : ControllerBase
    {
        private readonly IPremiumAssignmentService _premiumAssignmentService;

        public PremiumAssignmentController(IPremiumAssignmentService premiumAssignmentService)
        {
            _premiumAssignmentService = premiumAssignmentService;
        }

        /// <summary>
        /// Assigns premium status to a seller
        /// </summary>
        /// <param name="request">Assignment request data</param>
        /// <returns>Success response</returns>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignPremiumStatusAsync([FromBody] AssignPremiumRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adminId = Guid.Empty; // In a real implementation, this would come from the authenticated user
            var result = await _premiumAssignmentService.AssignPremiumStatusAsync(request.SellerId, adminId, request.ExpirationDate);

            if (!result)
            {
                return BadRequest(new { message = "Failed to assign premium status" });
            }

            return Ok(new { message = "Premium status assigned successfully" });
        }

        /// <summary>
        /// Removes premium status from a seller
        /// </summary>
        /// <param name="request">Removal request data</param>
        /// <returns>Success response</returns>
        [HttpPost("remove")]
        public async Task<IActionResult> RemovePremiumStatusAsync([FromBody] RemovePremiumRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adminId = Guid.Empty; // In a real implementation, this would come from the authenticated user
            var result = await _premiumAssignmentService.RemovePremiumStatusAsync(request.SellerId, adminId, request.Reason);

            if (!result)
            {
                return BadRequest(new { message = "Failed to remove premium status" });
            }

            return Ok(new { message = "Premium status removed successfully" });
        }

        /// <summary>
        /// Checks if a seller has premium status
        /// </summary>
        /// <param name="sellerId">ID of the seller to check</param>
        /// <returns>Premium status information</returns>
        [HttpGet("status/{sellerId}")]
        public async Task<ActionResult<PremiumStatusDto>> GetPremiumStatusAsync(Guid sellerId)
        {
            var status = await _premiumAssignmentService.GetPremiumStatusAsync(sellerId);
            if (status == null)
            {
                return NotFound();
            }
            return Ok(status);
        }

        /// <summary>
        /// Updates premium status expiration date
        /// </summary>
        /// <param name="sellerId">ID of the seller</param>
        /// <param name="request">Update request with new expiration date</param>
        /// <returns>Success response</returns>
        [HttpPut("expiration/{sellerId}")]
        public async Task<IActionResult> UpdatePremiumExpirationAsync(Guid sellerId, [FromBody] UpdateExpirationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _premiumAssignmentService.UpdatePremiumExpirationAsync(sellerId, request.ExpirationDate);

            if (!result)
            {
                return BadRequest(new { message = "Failed to update premium expiration" });
            }

            return Ok(new { message = "Premium expiration updated successfully" });
        }

        /// <summary>
        /// Gets all sellers with premium status
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="size">Page size (default: 10)</param>
        /// <returns>Paged list of premium sellers</returns>
        [HttpGet("sellers")]
        public async Task<ActionResult<PagedResultDto<PremiumStatusDto>>> GetPremiumSellersAsync([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _premiumAssignmentService.GetPremiumSellersAsync(page, size);
            return Ok(result);
        }
    }

    /// <summary>
    /// Request model for assigning premium status
    /// </summary>
    public class AssignPremiumRequest
    {
        public Guid SellerId { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    /// <summary>
    /// Request model for removing premium status
    /// </summary>
    public class RemovePremiumRequest
    {
        public Guid SellerId { get; set; }
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Request model for updating premium expiration
    /// </summary>
    public class UpdateExpirationRequest
    {
        public DateTime? ExpirationDate { get; set; }
    }
}