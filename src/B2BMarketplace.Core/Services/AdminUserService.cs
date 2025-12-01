using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Models;
using Microsoft.Extensions.Logging;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for admin user management operations
    /// </summary>
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly IBuyerProfileRepository _buyerProfileRepository;
        private readonly ICertificationRepository _certificationRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<AdminUserService> _logger;

        /// <summary>
        /// Constructor for AdminUserService
        /// </summary>
        /// <param name="userRepository">User repository</param>
        /// <param name="sellerProfileRepository">Seller profile repository</param>
        /// <param name="buyerProfileRepository">Buyer profile repository</param>
        /// <param name="certificationRepository">Certification repository</param>
        /// <param name="auditRepository">Audit repository</param>
        /// <param name="logger">Logger</param>
        public AdminUserService(
            IUserRepository userRepository,
            ISellerProfileRepository sellerProfileRepository,
            IBuyerProfileRepository buyerProfileRepository,
            ICertificationRepository certificationRepository,
            IAuditRepository auditRepository,
            ILogger<AdminUserService> logger)
        {
            _userRepository = userRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _buyerProfileRepository = buyerProfileRepository;
            _certificationRepository = certificationRepository;
            _auditRepository = auditRepository;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of users with optional search filter
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="search">Optional search term to filter users by email or name</param>
        /// <returns>Paged result containing users</returns>
        public async Task<PagedResult<User>> GetUsersAsync(int page, int pageSize, string? search = null)
        {
            return await _userRepository.GetUsersAsync(page, pageSize, search);
        }

        /// <summary>
        /// Locks a user account
        /// </summary>
        /// <param name="userId">ID of the user to lock</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <param name="reason">Reason for locking the user account</param>
        /// <param name="ipAddress">IP address of the admin</param>
        /// <param name="userAgent">User agent of the admin's browser/device</param>
        /// <returns>True if lock was successful, false otherwise</returns>
        public async Task<bool> LockUserAsync(Guid userId, Guid adminId, string reason, string? ipAddress = null, string? userAgent = null)
        {
            // Get the user to lock
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Check if user is already locked
            if (user.IsLocked)
            {
                return true; // Already locked, consider it successful
            }

            // Capture the user state before the action for AC-02
            var beforeValues = System.Text.Json.JsonSerializer.Serialize(new
            {
                IsLocked = user.IsLocked,
                LockReason = user.LockReason,
                LockDate = user.LockDate
            });

            // Update user properties for locking
            user.IsLocked = true;
            user.LockReason = reason;
            user.LockDate = DateTime.UtcNow;

            // Update the user in the database
            var updateResult = await _userRepository.UpdateUserAsync(user);
            if (!updateResult)
            {
                return false;
            }

            // Capture the user state after the action for AC-02
            var afterValues = System.Text.Json.JsonSerializer.Serialize(new
            {
                IsLocked = user.IsLocked,
                LockReason = user.LockReason,
                LockDate = user.LockDate
            });

            // Log the action in audit trail with required information for AC-01 and AC-02
            var auditLog = new UserManagementAuditLog();
            auditLog.InitializeAuditLog(
                userId: userId,
                adminId: adminId,
                action: "Lock",
                reason: reason,
                ipAddress: ipAddress,
                userAgent: userAgent,
                entityName: "User",
                entityId: userId.GetHashCode(), // Using hash code as integer ID for the GUID
                beforeValues: beforeValues,      // AC-02: state before action
                afterValues: afterValues         // AC-02: state after action
            );

            await _auditRepository.LogUserManagementActionAsync(auditLog);
            return true;
        }

        /// <summary>
        /// Unlocks a user account
        /// </summary>
        /// <param name="userId">ID of the user to unlock</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <param name="ipAddress">IP address of the admin</param>
        /// <param name="userAgent">User agent of the admin's browser/device</param>
        /// <returns>True if unlock was successful, false otherwise</returns>
        public async Task<bool> UnlockUserAsync(Guid userId, Guid adminId, string? ipAddress = null, string? userAgent = null)
        {
            // Get the user to unlock
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Check if user is already unlocked
            if (!user.IsLocked)
            {
                return true; // Already unlocked, consider it successful
            }

            // Capture the user state before the action for AC-02
            var beforeValues = System.Text.Json.JsonSerializer.Serialize(new
            {
                IsLocked = user.IsLocked,
                LockReason = user.LockReason,
                LockDate = user.LockDate
            });

            // Update user properties for unlocking
            user.IsLocked = false;
            user.LockReason = null;
            user.LockDate = null;

            // Update the user in the database
            var updateResult = await _userRepository.UpdateUserAsync(user);
            if (!updateResult)
            {
                return false;
            }

            // Capture the user state after the action for AC-02
            var afterValues = System.Text.Json.JsonSerializer.Serialize(new
            {
                IsLocked = user.IsLocked,
                LockReason = user.LockReason,
                LockDate = user.LockDate
            });

            // Log the action in audit trail with required information for AC-01 and AC-02
            var auditLog = new UserManagementAuditLog();
            auditLog.InitializeAuditLog(
                userId: userId,
                adminId: adminId,
                action: "Unlock",
                reason: "Account unlocked by admin",
                ipAddress: ipAddress,
                userAgent: userAgent,
                entityName: "User",
                entityId: userId.GetHashCode(), // Using hash code as integer ID for the GUID
                beforeValues: beforeValues,      // AC-02: state before action
                afterValues: afterValues         // AC-02: state after action
            );

            await _auditRepository.LogUserManagementActionAsync(auditLog);
            return true;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">User entity to create</param>
        /// <returns>Created user entity</returns>
        public async Task<User> CreateUserAsync(User user)
        {
            return await _userRepository.CreateUserAsync(user);
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="user">User entity to update</param>
        /// <returns>Updated user entity</returns>
        public async Task<User> UpdateUserAsync(User user)
        {
            // Update the UpdatedAt timestamp
            user.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _userRepository.UpdateUserAsync(user);
            if (!updateResult)
            {
                throw new Exception("Failed to update user");
            }

            return user;
        }

        /// <summary>
        /// Deletes a user by ID
        /// </summary>
        /// <param name="userId">ID of the user to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }

        /// <summary>
        /// Gets a user by ID
        /// </summary>
        /// <param name="userId">ID of the user to retrieve</param>
        /// <returns>User entity if found, null otherwise</returns>
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        /// <summary>
        /// Gets a user by email
        /// </summary>
        /// <param name="email">Email of the user to retrieve</param>
        /// <returns>User entity if found, null otherwise</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }
    }
}