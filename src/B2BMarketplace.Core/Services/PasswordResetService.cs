using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for password reset operations
    /// </summary>
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetTokenRepository _tokenRepository;
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;

        /// <summary>
        /// Constructor for PasswordResetService
        /// </summary>
        /// <param name="userRepository">User repository</param>
        /// <param name="tokenRepository">Token repository</param>
        /// <param name="emailService">Email service</param>
        /// <param name="passwordService">Password service</param>
        public PasswordResetService(
            IUserRepository userRepository,
            IPasswordResetTokenRepository tokenRepository,
            IEmailService emailService,
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Initiates the password reset process for a user with the specified email
        /// </summary>
        /// <param name="email">Email address of the user requesting password reset</param>
        /// <returns>True if the process was initiated successfully, false otherwise</returns>
        public async Task<bool> InitiatePasswordResetAsync(string email)
        {
            // Get user by email
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                // Return true for security - don't reveal if email exists
                return true;
            }

            // Expire any existing unused tokens for this user
            await _tokenRepository.ExpireUnusedTokensAsync(user.Id);

            // Generate a secure token
            var token = GenerateSecureToken();

            // Create password reset token
            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24) // 24 hour expiration
            };

            // Save token to database
            await _tokenRepository.CreateTokenAsync(resetToken);

            // Send password reset email
            await _emailService.SendPasswordResetEmailAsync(email, token);

            return true;
        }

        /// <summary>
        /// Validates a password reset token
        /// </summary>
        /// <param name="token">Token to validate</param>
        /// <returns>True if the token is valid, false otherwise</returns>
        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            var resetToken = await _tokenRepository.GetByTokenAsync(token);

            // Check if token exists and is not expired or used
            if (resetToken == null ||
                resetToken.Used ||
                resetToken.ExpiresAt < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Resets a user's password using a valid token
        /// </summary>
        /// <param name="token">Valid password reset token</param>
        /// <param name="newPassword">New password to set</param>
        /// <returns>True if the password was reset successfully, false otherwise</returns>
        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            // Validate password strength
            if (!IsPasswordStrong(newPassword))
            {
                return false;
            }

            // Get the reset token
            var resetToken = await _tokenRepository.GetByTokenAsync(token);

            // Check if token exists and is not expired or used
            if (resetToken == null ||
                resetToken.Used ||
                resetToken.ExpiresAt < DateTime.UtcNow)
            {
                return false;
            }

            // Get the user
            var user = await _userRepository.GetUserByIdAsync(resetToken.UserId);
            if (user == null)
            {
                return false;
            }

            // Update user's password
            user.PasswordHash = _passwordService.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // Update the user in the repository
            var updated = await _userRepository.UpdateUserAsync(user);
            if (!updated)
            {
                return false;
            }

            // Mark the token as used
            await _tokenRepository.MarkTokenAsUsedAsync(token);

            // Send confirmation email
            await _emailService.SendPasswordResetConfirmationEmailAsync(user.Email);

            return true;
        }

        /// <summary>
        /// Generates a cryptographically secure token
        /// </summary>
        /// <returns>Secure token string</returns>
        private static string GenerateSecureToken()
        {
            // Generate a 32-byte (256-bit) random token
            byte[] tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }

            // Convert to URL-safe base64 string
            return Convert.ToBase64String(tokenBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        /// <summary>
        /// Validates password strength
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>True if password is strong, false otherwise</returns>
        private static bool IsPasswordStrong(string password)
        {
            // Basic password strength validation
            // Password must be at least 8 characters long
            if (password.Length < 8)
                return false;

            // Password must contain at least one uppercase letter
            if (!password.Any(char.IsUpper))
                return false;

            // Password must contain at least one lowercase letter
            if (!password.Any(char.IsLower))
                return false;

            // Password must contain at least one digit
            if (!password.Any(char.IsDigit))
                return false;

            // Password must contain at least one special character
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                return false;

            return true;
        }
    }
}