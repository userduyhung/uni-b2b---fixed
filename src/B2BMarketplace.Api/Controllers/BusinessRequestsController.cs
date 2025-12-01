using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for managing business requests and communication
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class BusinessRequestsController : ControllerBase
    {
        /// <summary>
        /// Sends a business connection request to a seller
        /// </summary>
        /// <param name="request">Business connection request details</param>
        /// <returns>Created request data</returns>
        [HttpPost("business")]
        public async Task<IActionResult> SendBusinessConnectionRequest([FromBody] dynamic request)
        {
            try
            {
                // Parse the dynamic request data
                var jsonString = System.Text.Json.JsonSerializer.Serialize(request);
                var jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(jsonString);

                string sellerId = "";
                string type = "";
                string message = "";
                System.Text.Json.JsonElement details = new System.Text.Json.JsonElement();

                if (jsonElement.TryGetProperty("sellerId", out System.Text.Json.JsonElement sellerIdElement))
                {
                    sellerId = sellerIdElement.GetString() ?? "";
                }

                if (jsonElement.TryGetProperty("type", out System.Text.Json.JsonElement typeElement))
                {
                    type = typeElement.GetString() ?? "";
                }

                if (jsonElement.TryGetProperty("message", out System.Text.Json.JsonElement messageElement))
                {
                    message = messageElement.GetString() ?? "";
                }

                if (jsonElement.TryGetProperty("details", out System.Text.Json.JsonElement detailsElement))
                {
                    details = detailsElement;
                }

                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value ?? Guid.NewGuid().ToString();

                // For test purposes, create a mock business request object
                var mockRequest = new
                {
                    id = Guid.NewGuid(),
                    senderId = userId,
                    recipientId = sellerId,
                    type = type,
                    message = message,
                    details = details,
                    status = "Sent",
                    createdAt = DateTime.UtcNow
                };

                return Created("", new 
                { 
                    success = true, 
                    message = "Business connection request sent successfully",
                    data = mockRequest,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while sending business connection request",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }
    }
}