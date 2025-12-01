namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for email sending operations
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends a password reset email to the specified email address
        /// </summary>
        /// <param name="email">Recipient email address</param>
        /// <param name="resetToken">Password reset token to include in the email</param>
        /// <returns>True if email was sent successfully, false otherwise</returns>
        Task<bool> SendPasswordResetEmailAsync(string email, string resetToken);

        /// <summary>
        /// Sends a password reset confirmation email to the specified email address
        /// </summary>
        /// <param name="email">Recipient email address</param>
        /// <param name="userName">Name of the user (optional)</param>
        /// <returns>True if email was sent successfully, false otherwise</returns>
        Task<bool> SendPasswordResetConfirmationEmailAsync(string email, string? userName = null);
    }
}