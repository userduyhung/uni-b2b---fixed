using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface for password reset token data access operations
    /// </summary>
    public interface IPasswordResetTokenRepository
    {
        /// <summary>
        /// Creates a new password reset token in the database
        /// </summary>
        /// <param name="token">Password reset token to create</param>
        /// <returns>Created password reset token</returns>
        Task<PasswordResetToken> CreateTokenAsync(PasswordResetToken token);

        /// <summary>
        /// Gets a password reset token by its token string
        /// </summary>
        /// <param name="token">Token string to search for</param>
        /// <returns>Password reset token if found, null otherwise</returns>
        Task<PasswordResetToken?> GetByTokenAsync(string token);

        /// <summary>
        /// Expires all unused tokens for a user
        /// </summary>
        /// <param name="userId">User ID to expire tokens for</param>
        /// <returns>Number of tokens expired</returns>
        Task<int> ExpireUnusedTokensAsync(Guid userId);

        /// <summary>
        /// Marks a token as used
        /// </summary>
        /// <param name="token">Token string to mark as used</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> MarkTokenAsUsedAsync(string token);
    }
}