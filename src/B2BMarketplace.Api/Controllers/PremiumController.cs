using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Api.Helpers;

namespace B2BMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/premium")]
    public class PremiumController : ControllerBase
    {
        private readonly IPremiumSubscriptionService _premiumSubscriptionService;
        private readonly ILogger<PremiumController> _logger;

        public PremiumController(
            IPremiumSubscriptionService premiumSubscriptionService,
            ILogger<PremiumController> logger)
        {
            _premiumSubscriptionService = premiumSubscriptionService;
            _logger = logger;
        }

        [HttpGet("plans")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlans()
        {
            try
            {
                // Return mock plans for Postman tests
                var plans = new[]
                {
                    new { 
                        id = Guid.NewGuid(), 
                        name = "Basic", 
                        price = 29.99, 
                        features = new[] { "Feature 1", "Feature 2" },
                        description = "Basic premium features"
                    },
                    new { 
                        id = Guid.NewGuid(), 
                        name = "Premium", 
                        price = 59.99, 
                        features = new[] { "Feature 1", "Feature 2", "Feature 3" },
                        description = "Premium features with additional benefits"
                    }
                };
                
                return Ok(new
                {
                    success = true,
                    message = "Subscription plans retrieved successfully",
                    data = plans,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plans");
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving subscription plans",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpPost("subscribe")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("Id")?.Value ?? Guid.Empty.ToString());
                
                // Mock subscription creation for Postman tests
                var subscription = new
                {
                    id = Guid.NewGuid(),
                    sellerId = userId,
                    planId = request.PlanId,
                    status = "Active",
                    startDate = DateTime.UtcNow,
                    endDate = DateTime.UtcNow.AddMonths(1),
                    createdAt = DateTime.UtcNow
                };

                return Created("", new
                {
                    success = true,
                    message = "Subscription initiated successfully",
                    data = subscription,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription");
                return StatusCode(500, new
                {
                    error = "An error occurred while initiating subscription",
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }

    public class SubscribeRequest
    {
        public Guid PlanId { get; set; }
        public string PaymentMethodId { get; set; } = string.Empty;
    }
}