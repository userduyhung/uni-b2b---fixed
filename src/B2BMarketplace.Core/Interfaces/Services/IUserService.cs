using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for user registration and authentication operations
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user with the specified email, password, and role
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="password">User's password</param>
        /// <param name="role">User's role</param>
        /// <returns>Created user entity</returns>
        /// <exception cref="ArgumentException">Thrown when email is invalid or password is too weak</exception>
        /// <exception cref="InvalidOperationException">Thrown when user with email already exists</exception>
        Task<User> RegisterUserAsync(string email, string password, UserRole role);

        /// <summary>
        /// Authenticates a user with the specified email and password
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="password">User's password</param>
        /// <returns>User entity if authentication is successful, null otherwise</returns>
        Task<User?> AuthenticateUserAsync(string email, string password);

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="id">User ID to search for</param>
        /// <returns>User entity if found, null otherwise</returns>
        Task<User?> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Changes a user's password after verifying the current password
        /// </summary>
        /// <param name="userId">ID of the user whose password to change</param>
        /// <param name="currentPassword">Current password for verification</param>
        /// <param name="newPassword">New password to set</param>
        /// <returns>True if password was changed successfully, false otherwise</returns>
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    }
}