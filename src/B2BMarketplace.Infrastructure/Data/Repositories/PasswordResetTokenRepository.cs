using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for password reset token data access operations
    /// </summary>
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for PasswordResetTokenRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public PasswordResetTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new password reset token in the database
        /// </summary>
        /// <param name="token">Password reset token to create</param>
        /// <returns>Created password reset token</returns>
        public async Task<PasswordResetToken> CreateTokenAsync(PasswordResetToken token)
        {
            var entry = await _context.PasswordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Gets a password reset token by its token string
        /// </summary>
        /// <param name="token">Token string to search for</param>
        /// <returns>Password reset token if found, null otherwise</returns>
        public async Task<PasswordResetToken?> GetByTokenAsync(string token)
        {
            return await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == token);
        }

        /// <summary>
        /// Expires all unused tokens for a user
        /// </summary>
        /// <param name="userId">User ID to expire tokens for</param>
        /// <returns>Number of tokens expired</returns>
        public async Task<int> ExpireUnusedTokensAsync(Guid userId)
        {
            var tokens = await _context.PasswordResetTokens
                .Where(t => t.UserId == userId && !t.Used && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.ExpiresAt = DateTime.UtcNow.AddMinutes(-1); // Expire the token
                token.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return tokens.Count;
        }

        /// <summary>
        /// Marks a token as used
        /// </summary>
        /// <param name="token">Token string to mark as used</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> MarkTokenAsUsedAsync(string token)
        {
            var resetToken = await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (resetToken == null)
                return false;

            resetToken.Used = true;
            resetToken.UpdatedAt = DateTime.UtcNow;

            _context.PasswordResetTokens.Update(resetToken);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}