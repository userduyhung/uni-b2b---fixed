using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace B2BMarketplace.Api.Controllers;

/// <summary>
/// Base API controller for common endpoints
/// </summary>
[ApiController]
[Route("api")]
[Produces("application/json")]
public abstract class ApiController : ControllerBase
{
    /// <summary>
    /// Gets the current user ID from the claims
    /// </summary>
    protected Guid UserId
    {
        get
        {
            try
            {
                // Try different claim types that might contain the user ID
                var userIdClaim = User.FindFirst("nameid") ?? 
                                 User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ??
                                 User.FindFirst("sub") ??
                                 User.FindFirst("user_id");
                
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return userId;
                }
                
                // Log available claims for debugging
                var availableClaims = string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"));
                throw new UnauthorizedAccessException($"User ID not found in claims. Available claims: {availableClaims}");
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException($"Error retrieving user ID from claims: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        try
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Health check failed",
                message = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// API information endpoint
    /// </summary>
    /// <returns>API information</returns>
    [HttpGet("info")]
    public virtual IActionResult GetApiInfo()
    {
        try
        {
            return Ok(new
            {
                name = "B2B Marketplace API",
                version = "v1",
                description = "API for B2B Marketplace Platform",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "API info retrieval failed",
                message = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// API version endpoint
    /// </summary>
    /// <returns>Version information</returns>
    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        try
        {
            return Ok(new
            {
                version = "v1",
                buildDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                apiVersion = "1.0.0"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Version info retrieval failed",
                message = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}