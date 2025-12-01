using BCrypt.Net;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for password hashing and validation operations using BCrypt
    /// </summary>
    public class PasswordService : IPasswordService
    {
        /// <summary>
        /// Hashes a password using BCrypt
        /// </summary>
        /// <param name="password">Plain text password to hash</param>
        /// <returns>Hashed password</returns>
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Validates a plain text password against a hashed password using BCrypt
        /// </summary>
        /// <param name="password">Plain text password to validate</param>
        /// <param name="hashedPassword">Hashed password to compare against</param>
        /// <returns>True if passwords match, false otherwise</returns>
        public bool ValidatePassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (string.IsNullOrEmpty(hashedPassword))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}