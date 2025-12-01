using System.Security.Claims;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Core.Services
{
    public class ClaimsUtilityService : IClaimsUtilityService
    {
        /// <summary>
        /// Gets the user ID from the ClaimsPrincipal
        /// </summary>
        /// <param name="user">ClaimsPrincipal containing user claims</param>
        /// <returns>User ID as string, or null if not found</returns>
        public string GetUserIdFromClaims(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("nameid")?.Value ?? 
                             user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim;
        }

        /// <summary>
        /// Gets the user role from the ClaimsPrincipal
        /// </summary>
        /// <param name="user">ClaimsPrincipal containing user claims</param>
        /// <returns>User role as string, or null if not found</returns>
        public string GetUserRoleFromClaims(ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst("role")?.Value ?? 
                           user.FindFirst(ClaimTypes.Role)?.Value;
            return roleClaim;
        }

        /// <summary>
        /// Gets the user email from the ClaimsPrincipal
        /// </summary>
        /// <param name="user">ClaimsPrincipal containing user claims</param>
        /// <returns>User email as string, or null if not found</returns>
        public string GetUserEmailFromClaims(ClaimsPrincipal user)
        {
            var emailClaim = user.FindFirst("email")?.Value ?? 
                            user.FindFirst(ClaimTypes.Email)?.Value;
            return emailClaim;
        }
    }
}