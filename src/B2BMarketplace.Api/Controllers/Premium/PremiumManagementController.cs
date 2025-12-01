using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services.Premium;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BMarketplace.Api.Controllers.Premium
{
    /// <summary>
    /// Controller for premium management operations
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin/premium/management")]
    public class PremiumManagementController : ControllerBase
    {
        private readonly IPremiumManagementService _premiumManagementService;

        public PremiumManagementController(IPremiumManagementService premiumManagementService)
        {
            _premiumManagementService = premiumManagementService;
        }

        /// <summary>
        /// Gets all premium service tiers
        /// </summary>
        /// <returns>List of service tiers</returns>
        [HttpGet("service-tiers")]
        public async Task<ActionResult<IEnumerable<ServiceTierDto>>> GetServiceTiersAsync()
        {
            var tiers = await _premiumManagementService.GetServiceTiersAsync();
            return Ok(tiers);
        }

        /// <summary>
        /// Gets a service tier by ID
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>Service tier details</returns>
        [HttpGet("service-tiers/{id}")]
        public async Task<ActionResult<ServiceTierDto>> GetServiceTierByIdAsync(Guid id)
        {
            var tier = await _premiumManagementService.GetServiceTierByIdAsync(id);
            if (tier == null)
            {
                return NotFound();
            }
            return Ok(tier);
        }

        /// <summary>
        /// Creates a new service tier
        /// </summary>
        /// <param name="serviceTierDto">Service tier data</param>
        /// <returns>Created service tier</returns>
        [HttpPost("service-tiers")]
        public async Task<ActionResult<ServiceTierDto>> CreateServiceTierAsync([FromBody] ServiceTierDto serviceTierDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdTier = await _premiumManagementService.CreateServiceTierAsync(serviceTierDto);
            return CreatedAtAction(nameof(GetServiceTierByIdAsync), new { id = createdTier.Id }, createdTier);
        }

        /// <summary>
        /// Updates an existing service tier
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <param name="serviceTierDto">Updated service tier data</param>
        /// <returns>Updated service tier</returns>
        [HttpPut("service-tiers/{id}")]
        public async Task<ActionResult<ServiceTierDto>> UpdateServiceTierAsync(Guid id, [FromBody] ServiceTierDto serviceTierDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedTier = await _premiumManagementService.UpdateServiceTierAsync(id, serviceTierDto);
            if (updatedTier == null)
            {
                return NotFound();
            }
            return Ok(updatedTier);
        }

        /// <summary>
        /// Deletes a service tier by ID
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("service-tiers/{id}")]
        public async Task<IActionResult> DeleteServiceTierAsync(Guid id)
        {
            var result = await _premiumManagementService.DeleteServiceTierAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Gets premium subscription history for a seller
        /// </summary>
        /// <param name="sellerId">ID of the seller</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="size">Page size (default: 10)</param>
        /// <returns>Paged subscription history</returns>
        [HttpGet("subscription-history/{sellerId}")]
        public async Task<ActionResult<PagedResultDto<SubscriptionHistoryDto>>> GetSubscriptionHistoryAsync(Guid sellerId, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _premiumManagementService.GetSubscriptionHistoryAsync(sellerId, page, size);
            return Ok(result);
        }

        /// <summary>
        /// Gets premium seller analytics
        /// </summary>
        /// <param name="startDate">Start date for analytics</param>
        /// <param name="endDate">End date for analytics</param>
        /// <returns>Premium analytics data</returns>
        [HttpGet("analytics")]
        public async Task<ActionResult<PremiumAnalyticsDto>> GetPremiumAnalyticsAsync([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var analytics = await _premiumManagementService.GetPremiumAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }

        /// <summary>
        /// Processes premium renewal for a seller
        /// </summary>
        /// <param name="request">Renewal request data</param>
        /// <returns>Success response</returns>
        [HttpPost("renewal")]
        public async Task<IActionResult> ProcessRenewalAsync([FromBody] RenewalRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _premiumManagementService.ProcessRenewalAsync(request.SellerId, request.NewTierId);

            if (!result)
            {
                return BadRequest(new { message = "Failed to process renewal" });
            }

            return Ok(new { message = "Renewal processed successfully" });
        }
    }

    /// <summary>
    /// Request model for processing renewal
    /// </summary>
    public class RenewalRequest
    {
        public Guid SellerId { get; set; }
        public Guid NewTierId { get; set; }
    }
}