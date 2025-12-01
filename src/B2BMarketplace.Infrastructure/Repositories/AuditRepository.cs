using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for user management audit log data access operations
    /// </summary>
    public class AuditRepository : IAuditRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Logs a user management action performed by an admin
        /// </summary>
        /// <param name="auditLog">Audit log entry to create</param>
        /// <returns>Created audit log entry</returns>
        public async Task<UserManagementAuditLog> LogUserManagementActionAsync(UserManagementAuditLog auditLog)
        {
            _context.UserManagementAuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
            auditLog.MarkAsCreated(); // Mark the audit log as created to enforce immutability (AC-03)
            return auditLog;
        }
    }
}