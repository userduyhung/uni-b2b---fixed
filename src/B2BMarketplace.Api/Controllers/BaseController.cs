using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly ITokenService _tokenService;

        public BaseController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Gets the user ID from the JWT token claims
        /// </summary>
        /// <returns>User ID as string, or null if not found</returns>
        protected string? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst("nameid")?.Value ?? 
                             User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim;
        }

        /// <summary>
        /// Gets the user role from the JWT token claims
        /// </summary>
        /// <returns>User role as string, or null if not found</returns>
        protected string? GetUserRoleFromClaims()
        {
            var roleClaim = User.FindFirst("role")?.Value ?? 
                           User.FindFirst(ClaimTypes.Role)?.Value;
            return roleClaim;
        }

        /// <summary>
        /// Gets the user email from the JWT token claims
        /// </summary>
        /// <returns>User email as string, or null if not found</returns>
        protected string? GetUserEmailFromClaims()
        {
            var emailClaim = User.FindFirst("email")?.Value ?? 
                            User.FindFirst(ClaimTypes.Email)?.Value;
            return emailClaim;
        }
    }
}