using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for JWT token generation and validation operations
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        /// <param name="user">User to generate token for</param>
        /// <returns>JWT token string</returns>
        string GenerateToken(User user);

        /// <summary>
        /// Validates a JWT token and returns the user ID if valid
        /// </summary>
        /// <param name="token">JWT token to validate</param>
        /// <returns>User ID if token is valid, null otherwise</returns>
        Guid? ValidateToken(string token);
    }
}