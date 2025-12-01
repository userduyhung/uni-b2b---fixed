using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a password reset token for secure password recovery
    /// </summary>
    public class PasswordResetToken
    {
        /// <summary>
        /// Unique identifier for the token
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the user requesting password reset
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Secure token string for password reset
        /// </summary>
        [Required]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the token was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the token expires
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Whether the token has been used
        /// </summary>
        public bool Used { get; set; }

        /// <summary>
        /// Timestamp when the token was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Navigation property to the user
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public PasswordResetToken()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Used = false;
        }
    }
}