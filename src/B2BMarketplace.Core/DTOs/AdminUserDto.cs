namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for user information in admin user management
    /// </summary>
    public class AdminUserDto
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's role in the system
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when user was created
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Timestamp of the user's last login
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Whether the user account is locked
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Reason for locking the user account
        /// </summary>
        public string? LockReason { get; set; }

        /// <summary>
        /// Timestamp when the user account was locked
        /// </summary>
        public DateTime? LockDate { get; set; }
    }
}