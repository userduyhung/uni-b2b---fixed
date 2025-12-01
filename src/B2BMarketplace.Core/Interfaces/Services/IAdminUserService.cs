using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for admin user management operations
    /// </summary>
    public interface IAdminUserService
    {
        /// <summary>
        /// Gets a paginated list of users with optional search filter
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="search">Optional search term to filter users by email or name</param>
        /// <returns>Paged result containing users</returns>
        Task<PagedResult<User>> GetUsersAsync(int page, int pageSize, string? search = null);

        /// <summary>
        /// Locks a user account
        /// </summary>
        /// <param name="userId">ID of the user to lock</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <param name="reason">Reason for locking the user account</param>
        /// <param name="ipAddress">IP address of the admin</param>
        /// <param name="userAgent">User agent of the admin's browser/device</param>
        /// <returns>True if lock was successful, false otherwise</returns>
        Task<bool> LockUserAsync(Guid userId, Guid adminId, string reason, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Unlocks a user account
        /// </summary>
        /// <param name="userId">ID of the user to unlock</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <param name="ipAddress">IP address of the admin</param>
        /// <param name="userAgent">User agent of the admin's browser/device</param>
        /// <returns>True if unlock was successful, false otherwise</returns>
        Task<bool> UnlockUserAsync(Guid userId, Guid adminId, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">User entity to create</param>
        /// <returns>Created user entity</returns>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="user">User entity to update</param>
        /// <returns>Updated user entity</returns>
        Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// Deletes a user by ID
        /// </summary>
        /// <param name="userId">ID of the user to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteUserAsync(Guid userId);

        /// <summary>
        /// Gets a user by ID
        /// </summary>
        /// <param name="userId">ID of the user to retrieve</param>
        /// <returns>User entity if found, null otherwise</returns>
        Task<User?> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// Gets a user by email
        /// </summary>
        /// <param name="email">Email of the user to retrieve</param>
        /// <returns>User entity if found, null otherwise</returns>
        Task<User?> GetUserByEmailAsync(string email);
    }
}