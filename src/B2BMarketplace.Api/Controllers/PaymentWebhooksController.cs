using Microsoft.AspNetCore.Mvc;
using B2BMarketplace.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for handling payment provider webhooks
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [AllowAnonymous] // Webhooks are typically not authenticated with JWT but with provider-specific signatures
    public class PaymentWebhooksController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentWebhooksController> _logger;

        /// <summary>
        /// Constructor for PaymentWebhooksController
        /// </summary>
        /// <param name="paymentService">Payment service</param>
        /// <param name="logger">Logger</param>
        public PaymentWebhooksController(IPaymentService paymentService, ILogger<PaymentWebhooksController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Handles payment confirmation webhook from payment providers
        /// </summary>
        /// <param name="webhookData">Webhook data from payment provider</param>
        /// <returns>Success response</returns>
        [HttpPost("payment-confirmation")]
        public async Task<IActionResult> HandlePaymentConfirmation([FromBody] PaymentWebhookDto webhookData)
        {
            try
            {
                // In a real implementation, you would:
                // 1. Verify the webhook signature to ensure it's from a trusted payment provider
                // 2. Parse the webhook data according to the provider's format
                // 3. Extract the payment ID, status, and other relevant information

                // For this example, we'll assume the webhook data contains:
                // - PaymentId: The ID of the payment in our system
                // - IsSuccessful: Whether the payment was successful
                // - ProviderTransactionId: The transaction ID from the payment provider
                // - ErrorMessage: Error message if payment failed (optional)

                _logger.LogInformation("Processing payment confirmation webhook for payment {PaymentId}", webhookData.PaymentId);

                var result = await _paymentService.ProcessPaymentConfirmationAsync(
                    webhookData.PaymentId,
                    false, // Default value for isSuccessful since it's not in the DTO
                    null,  // Default value for providerTransactionId since it's not in the DTO
                    null); // Default value for errorMessage since it's not in the DTO

                if (result)
                {
                    _logger.LogInformation("Successfully processed payment confirmation webhook for payment {PaymentId}", webhookData.PaymentId);
                    return Ok(new
                    {
                        message = "Payment confirmation processed successfully",
                        timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    _logger.LogWarning("Failed to process payment confirmation webhook for payment {PaymentId}", webhookData.PaymentId);
                    return BadRequest(new
                    {
                        error = "Failed to process payment confirmation",
                        timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment confirmation webhook");
                return StatusCode(500, new
                {
                    error = "An error occurred while processing the payment confirmation webhook",
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}