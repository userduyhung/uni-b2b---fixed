using B2BMarketplace.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace B2BMarketplace.Infrastructure.Services
{
    /// <summary>
    /// Mock email service for sending emails
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        /// <summary>
        /// Constructor for EmailService
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sends a password reset email to the specified email address
        /// </summary>
        /// <param name="email">Recipient email address</param>
        /// <param name="resetToken">Password reset token to include in the email</param>
        /// <returns>True if email was sent successfully, false otherwise</returns>
        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetToken)
        {
            try
            {
                // In a real implementation, this would send an actual email
                // For now, we'll just log the attempt
                _logger.LogInformation("Password reset email sent to {Email} with token {Token}", email, resetToken);

                // Simulate network delay
                await Task.Delay(100);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                return false;
            }
        }

        /// <summary>
        /// Sends a password reset confirmation email to the specified email address
        /// </summary>
        /// <param name="email">Recipient email address</param>
        /// <param name="userName">Name of the user (optional)</param>
        /// <returns>True if email was sent successfully, false otherwise</returns>
        public async Task<bool> SendPasswordResetConfirmationEmailAsync(string email, string? userName = null)
        {
            try
            {
                // In a real implementation, this would send an actual email
                // For now, we'll just log the attempt
                _logger.LogInformation("Password reset confirmation email sent to {Email}", email);

                // Simulate network delay
                await Task.Delay(100);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset confirmation email to {Email}", email);
                return false;
            }
        }
    }
}