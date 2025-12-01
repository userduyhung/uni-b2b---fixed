using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for user registration and authentication operations
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Constructor for UserService
        /// </summary>
        /// <param name="userRepository">User repository</param>
        /// <param name="passwordService">Password service</param>
        /// <param name="tokenService">Token service</param>
        public UserService(IUserRepository userRepository, IPasswordService passwordService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registers a new user with the specified email, password, and role
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="password">User's password</param>
        /// <param name="role">User's role</param>
        /// <returns>Created user entity</returns>
        /// <exception cref="ArgumentException">Thrown when email is invalid or password is too weak</exception>
        /// <exception cref="InvalidOperationException">Thrown when user with email already exists</exception>
        public async Task<User> RegisterUserAsync(string email, string password, UserRole role)
        {
            // Validate input
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email is required", nameof(email));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password is required", nameof(password));

            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long", nameof(password));

            // Check if user already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
                throw new InvalidOperationException($"User with email '{email}' already exists");

            // Hash password
            var hashedPassword = _passwordService.HashPassword(password);

            // Create user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = hashedPassword,
                Role = role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save user
            await _userRepository.CreateUserAsync(user);

            return user;
        }

        /// <summary>
        /// Authenticates a user with the specified email and password
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="password">User's password</param>
        /// <returns>User entity if authentication is successful, null otherwise</returns>
        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            // Validate input
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            // Get user by email
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return null;

            // Verify password
            if (!_passwordService.ValidatePassword(password, user.PasswordHash))
                return null;

            return user;
        }

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="id">User ID to search for</param>
        /// <returns>User entity if found, null otherwise</returns>
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        /// <summary>
        /// Changes a user's password after verifying the current password
        /// </summary>
        /// <param name="userId">ID of the user whose password to change</param>
        /// <param name="currentPassword">Current password for verification</param>
        /// <param name="newPassword">New password to set</param>
        /// <returns>True if password was changed successfully, false otherwise</returns>
        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            // Validate input parameters
            if (!ValidatePasswordChangeInput(currentPassword, newPassword))
                return false;

            // Retrieve user from repository
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return false;

            // Verify the current password matches the stored hash
            if (!VerifyCurrentPasswordAsync(currentPassword, user.PasswordHash))
                return false;

            // Ensure new password is different from current password
            if (IsNewPasswordSameAsCurrentAsync(newPassword, user.PasswordHash))
                return false;

            // Validate new password strength requirements
            if (!IsPasswordStrong(newPassword))
                return false;

            // Update user's password with the new hashed value
            var newHashedPassword = _passwordService.HashPassword(newPassword);
            user.PasswordHash = newHashedPassword;
            user.UpdatedAt = DateTime.UtcNow;

            // Save updated user to repository
            return await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Validates the input parameters for a password change operation
        /// </summary>
        /// <param name="currentPassword">Current password provided by user</param>
        /// <param name="newPassword">New password to set</param>
        /// <returns>True if inputs are valid, false otherwise</returns>
        private static bool ValidatePasswordChangeInput(string currentPassword, string newPassword)
        {
            return !string.IsNullOrWhiteSpace(currentPassword) &&
                   !string.IsNullOrWhiteSpace(newPassword);
        }

        /// <summary>
        /// Verifies that the provided current password matches the stored password hash
        /// </summary>
        /// <param name="currentPassword">Password provided by user</param>
        /// <param name="storedPasswordHash">Hash of the stored password</param>
        /// <returns>True if passwords match, false otherwise</returns>
        private bool VerifyCurrentPasswordAsync(string currentPassword, string storedPasswordHash)
        {
            return _passwordService.ValidatePassword(currentPassword, storedPasswordHash);
        }

        /// <summary>
        /// Checks if the new password is the same as the current password
        /// </summary>
        /// <param name="newPassword">New password to check</param>
        /// <param name="storedPasswordHash">Hash of the current password</param>
        /// <returns>True if passwords are the same, false otherwise</returns>
        private bool IsNewPasswordSameAsCurrentAsync(string newPassword, string storedPasswordHash)
        {
            return _passwordService.ValidatePassword(newPassword, storedPasswordHash);
        }

        /// <summary>
        /// Validates that a password meets strength requirements
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>True if password is strong, false otherwise</returns>
        private static bool IsPasswordStrong(string password)
        {
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