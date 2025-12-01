namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for password reset operations
    /// </summary>
    public interface IPasswordResetService
    {
        /// <summary>
        /// Initiates the password reset process for a user with the specified email
        /// </summary>
        /// <param name="email">Email address of the user requesting password reset</param>
        /// <returns>True if the process was initiated successfully, false otherwise</returns>
        Task<bool> InitiatePasswordResetAsync(string email);

        /// <summary>
        /// Validates a password reset token
        /// </summary>
        /// <param name="token">Token to validate</param>
        /// <returns>True if the token is valid, false otherwise</returns>
        Task<bool> ValidateResetTokenAsync(string token);

        /// <summary>
        /// Resets a user's password using a valid token
        /// </summary>
        /// <param name="token">Valid password reset token</param>
        /// <param name="newPassword">New password to set</param>
        /// <returns>True if the password was reset successfully, false otherwise</returns>
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}