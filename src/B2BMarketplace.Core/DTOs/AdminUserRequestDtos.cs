using System;
using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// DTO for creating a user
    /// </summary>
    public class CreateUserDto
    {
        /// <summary>
        /// User's email address
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's password
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// User's role
        /// </summary>
        [Required]
        public Core.Enums.UserRole Role { get; set; }
    }

    /// <summary>
    /// DTO for updating a user
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// User's email address
        /// </summary>
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Whether the user account is active
        /// </summary>
        public bool IsActive { get; set; }
    }
}