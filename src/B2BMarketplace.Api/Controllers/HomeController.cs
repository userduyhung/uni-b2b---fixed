using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace B2BMarketplace.Api.Controllers;

/// <summary>
/// Default API controller for basic operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
public class HomeController : ControllerBase
{
    /// <summary>
    /// Get API status
    /// </summary>
    /// <returns>API status information</returns>
    [HttpGet]
    [OutputCache]
    public IActionResult Get()
    {
        return Ok(new
        {
            message = "B2B Marketplace API is running",
            version = "v1",
            timestamp = DateTime.UtcNow,
            status = "healthy"
        });
    }

    /*
    /// <summary>
    /// Get API information
    /// </summary>
    /// <returns>API information</returns>
    [HttpGet("info")]
    [OutputCache]
    public IActionResult GetInfo()
    {
        return Ok(new
        {
            title = "B2B Marketplace API",
            version = "v1",
            description = "API for B2B Marketplace Platform",
            timestamp = DateTime.UtcNow
        });
    }
    */
}