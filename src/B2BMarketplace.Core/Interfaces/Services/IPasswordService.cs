namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for password hashing and validation operations
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Hashes a password using a secure hashing algorithm
        /// </summary>
        /// <param name="password">Plain text password to hash</param>
        /// <returns>Hashed password</returns>
        string HashPassword(string password);

        /// <summary>
        /// Validates a plain text password against a hashed password
        /// </summary>
        /// <param name="password">Plain text password to validate</param>
        /// <param name="hashedPassword">Hashed password to compare against</param>
        /// <returns>True if passwords match, false otherwise</returns>
        bool ValidatePassword(string password, string hashedPassword);
    }
}