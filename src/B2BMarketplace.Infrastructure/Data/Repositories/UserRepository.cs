using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using B2BMarketplace.Core.Models;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for user data access operations
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for UserRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new user in the database
        /// </summary>
        /// <param name="user">User entity to create</param>
        /// <returns>Created user entity</returns>
        public async Task<User> CreateUserAsync(User user)
        {
            var entry = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Gets a user by their email address
        /// </summary>
        /// <param name="email">Email address to search for</param>
        /// <returns>User entity if found, null otherwise</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="id">User ID to search for</param>
        /// <returns>User entity if found, null otherwise</returns>
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Checks if a user with the specified email already exists
        /// </summary>
        /// <param name="email">Email address to check</param>
        /// <returns>True if user exists, false otherwise</returns>
        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Gets a paginated list of users with optional search filter
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="search">Optional search term to filter users by email</param>
        /// <returns>Paged result containing users</returns>
        public async Task<PagedResult<User>> GetUsersAsync(int page, int pageSize, string? search = null)
        {
            // Ensure page and pageSize are valid
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit max page size

            // Build query
            var query = _context.Users.AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Email.Contains(search));
            }

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User>(items, totalItems, page, pageSize);
        }

        /// <summary>
        /// Updates an existing user in the database
        /// </summary>
        /// <param name="user">User entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the count of users with the specified role
        /// </summary>
        /// <param name="role">User role to count</param>
        /// <returns>Number of users with the specified role</returns>
        public async Task<int> GetCountByRoleAsync(UserRole role)
        {
            return await _context.Users.CountAsync(u => u.Role == role);
        }

        /// <summary>
        /// Gets the count of users created up to the specified date
        /// </summary>
        /// <param name="date">Date to count users up to</param>
        /// <returns>Number of users created up to the specified date</returns>
        public async Task<int> GetCountByDateAsync(DateTime date)
        {
            return await _context.Users.CountAsync(u => u.CreatedAt <= date);
        }

        /// <summary>
        /// Gets users created in the specified date range
        /// </summary>
        /// <param name="startDate">Start date for the range</param>
        /// <param name="endDate">End date for the range</param>
        /// <returns>List of users created in the date range</returns>
        public async Task<IEnumerable<User>> GetUsersCreatedInDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Users
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the total count of users
        /// </summary>
        /// <returns>Total number of users</returns>
        public async Task<int> GetTotalUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        /// <summary>
        /// Deletes a user by ID
        /// </summary>
        /// <param name="id">User ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Locks a user account
        /// </summary>
        /// <param name="userId">ID of the user to lock</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <param name="reason">Reason for locking the user account</param>
        /// <returns>True if lock was successful, false otherwise</returns>
        public async Task<bool> LockUserAsync(Guid userId, Guid adminId, string reason)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.IsLocked = true;
            user.LockReason = reason;
            user.LockDate = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Unlocks a user account
        /// </summary>
        /// <param name="userId">ID of the user to unlock</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <returns>True if unlock was successful, false otherwise</returns>
        public async Task<bool> UnlockUserAsync(Guid userId, Guid adminId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.IsLocked = false;
            user.LockReason = null;
            user.LockDate = null;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}