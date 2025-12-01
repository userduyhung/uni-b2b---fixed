using System.Security.Claims;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface IClaimsUtilityService
    {
        /// <summary>
        /// Gets the user ID from the ClaimsPrincipal
        /// </summary>
        /// <param name="user">ClaimsPrincipal containing user claims</param>
        /// <returns>User ID as string, or null if not found</returns>
        string GetUserIdFromClaims(ClaimsPrincipal user);

        /// <summary>
        /// Gets the user role from the ClaimsPrincipal
        /// </summary>
        /// <param name="user">ClaimsPrincipal containing user claims</param>
        /// <returns>User role as string, or null if not found</returns>
        string GetUserRoleFromClaims(ClaimsPrincipal user);

        /// <summary>
        /// Gets the user email from the ClaimsPrincipal
        /// </summary>
        /// <param name="user">ClaimsPrincipal containing user claims</param>
        /// <returns>User email as string, or null if not found</returns>
        string GetUserEmailFromClaims(ClaimsPrincipal user);
    }
}