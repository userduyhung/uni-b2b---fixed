using System.ComponentModel.DataAnnotations;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a user in the B2B Marketplace platform
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User's email address (unique)
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hashed password for authentication
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// User's role in the system (Buyer, Seller, Admin)
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Timestamp when user was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when user was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Whether the user account is active
        /// </summary>
        public bool IsActive { get; set; }

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

        /// <summary>
        /// Timestamp when the user account lockout ends
        /// </summary>
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// Timestamp of the user's last login
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Navigation property to buyer profile (if user is a buyer)
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public BuyerProfile? BuyerProfile { get; set; }

        /// <summary>
        /// Navigation property to seller profile (if user is a seller)
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public SellerProfile? SellerProfile { get; set; }

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public User()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            IsLocked = false;
        }
    }
}