using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Public API controller that exposes general API endpoints
    /// </summary>
    [ApiController]
    [Route("api")]
    [Produces("application/json")]
    public class PublicApiController : ControllerBase
    {
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
}