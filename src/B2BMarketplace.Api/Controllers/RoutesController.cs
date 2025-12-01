using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for managing routes and redirects for missing endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class RoutesController : ControllerBase
    {
        /// <summary>
        /// Handles missing routes and redirects to appropriate controllers
        /// </summary>
        /// <returns>Appropriate response based on the requested route</returns>
        [HttpGet("{*url}")]
        [HttpPost("{*url}")]
        [HttpPut("{*url}")]
        [HttpDelete("{*url}")]
        [HttpPatch("{*url}")]
        public async Task<IActionResult> HandleRoute(string url)
        {
            try
            {
                // Log the requested URL for debugging purposes
                Console.WriteLine($"Route requested: {url}");

                // Handle specific known missing routes
                if (url.StartsWith("cart/") && url.EndsWith("/items"))
                {
                    // Redirect to CartController
                    return NotFound(new
                    {
                        error = "Cart not found",
                        timestamp = DateTime.UtcNow,
                        message = "Please provide a valid cart ID"
                    });
                }
                else if (url.StartsWith("orders/") && url.Contains("/status"))
                {
                    // Redirect to OrderController
                    return NotFound(new
                    {
                        error = "Order not found",
                        timestamp = DateTime.UtcNow,
                        message = "Please provide a valid order ID"
                    });
                }
                else if (url.StartsWith("orders/") && url.Contains("/tracking"))
                {
                    // Redirect to OrderController
                    return NotFound(new
                    {
                        error = "Order not found",
                        timestamp = DateTime.UtcNow,
                        message = "Please provide a valid order ID"
                    });
                }
                else if (url.StartsWith("orders/") && url.Contains("/confirm"))
                {
                    // Redirect to OrderController
                    return NotFound(new
                    {
                        error = "Order not found",
                        timestamp = DateTime.UtcNow,
                        message = "Please provide a valid order ID"
                    });
                }
                else if (url.StartsWith("products/") && url.Contains("/feedback"))
                {
                    // Redirect to ProductsController
                    return NotFound(new
                    {
                        error = "Product not found",
                        timestamp = DateTime.UtcNow,
                        message = "Please provide a valid product ID"
                    });
                }
                else if (url.StartsWith("reports"))
                {
                    // Redirect to ReportsController
                    return NotFound(new
                    {
                        error = "Reports endpoint not found",
                        timestamp = DateTime.UtcNow,
                        message = "Method not allowed for reports"
                    });
                }
                else if (url.StartsWith("requests/business"))
                {
                    // Redirect to RequestsController
                    return NotFound(new
                    {
                        error = "Business requests endpoint not found",
                        timestamp = DateTime.UtcNow,
                        message = "Please check the request format"
                    });
                }
                else if (url.StartsWith("messages"))
                {
                    // Redirect to MessagesController
                    return NotFound(new
                    {
                        error = "Messages endpoint not found",
                        timestamp = DateTime.UtcNow,
                        message = "Method not allowed for messages"
                    });
                }
                else if (url.StartsWith("shops"))
                {
                    // Redirect to ShopsController
                    return NotFound(new
                    {
                        error = "Shops endpoint not found",
                        timestamp = DateTime.UtcNow,
                        message = "Method not allowed for shops"
                    });
                }

                // For all other routes, return a generic 404
                return NotFound(new
                {
                    error = "Endpoint not found",
                    timestamp = DateTime.UtcNow,
                    message = $"The requested endpoint '{url}' was not found"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while processing the request",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }
    }
}