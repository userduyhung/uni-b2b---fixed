using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface for user data access operations
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Creates a new user in the database
        /// </summary>
        /// <param name="user">User entity to create</param>
        /// <returns>Created user entity</returns>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Gets a user by their email address
        /// </summary>
        /// <param name="email">Email address to search for</param>
        /// <returns>User entity if found, null otherwise</returns>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="id">User ID to search for</param>
        /// <returns>User entity if found, null otherwise</returns>
        Task<User?> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Checks if a user with the specified email already exists
        /// </summary>
        /// <param name="email">Email address to check</param>
        /// <returns>True if user exists, false otherwise</returns>
        Task<bool> UserExistsAsync(string email);

        /// <summary>
        /// Gets a paginated list of users with optional search filter
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="search">Optional search term to filter users by email or name</param>
        /// <returns>Paged result containing users</returns>
        Task<PagedResult<User>> GetUsersAsync(int page, int pageSize, string? search = null);

        /// <summary>
        /// Updates an existing user in the database
        /// </summary>
        /// <param name="user">User entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// Gets the count of users with the specified role
        /// </summary>
        /// <param name="role">User role to count</param>
        /// <returns>Number of users with the specified role</returns>
        Task<int> GetCountByRoleAsync(UserRole role);

        /// <summary>
        /// Gets the count of users created up to the specified date
        /// </summary>
        /// <param name="date">Date to count users up to</param>
        /// <returns>Number of users created up to the specified date</returns>
        Task<int> GetCountByDateAsync(DateTime date);

        /// <summary>
        /// Gets users created in the specified date range
        /// </summary>
        /// <param name="startDate">Start date for the range</param>
        /// <param name="endDate">End date for the range</param>
        /// <returns>List of users created in the date range</returns>
        Task<IEnumerable<User>> GetUsersCreatedInDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets the total count of users
        /// </summary>
        /// <returns>Total number of users</returns>
        Task<int> GetTotalUserCountAsync();

        /// <summary>
        /// Deletes a user by ID
        /// </summary>
        /// <param name="id">User ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteUserAsync(Guid id);

        /// <summary>
        /// Locks a user account
        /// </summary>
        /// <param name="userId">ID of the user to lock</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <param name="reason">Reason for locking the user account</param>
        /// <returns>True if lock was successful, false otherwise</returns>
        Task<bool> LockUserAsync(Guid userId, Guid adminId, string reason);

        /// <summary>
        /// Unlocks a user account
        /// </summary>
        /// <param name="userId">ID of the user to unlock</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <returns>True if unlock was successful, false otherwise</returns>
        Task<bool> UnlockUserAsync(Guid userId, Guid adminId);
    }
}