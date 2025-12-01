using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface for user management audit log data access operations
    /// </summary>
    public interface IAuditRepository
    {
        /// <summary>
        /// Logs a user management action performed by an admin
        /// </summary>
        /// <param name="auditLog">Audit log entry to create</param>
        /// <returns>Created audit log entry</returns>
        Task<UserManagementAuditLog> LogUserManagementActionAsync(UserManagementAuditLog auditLog);
    }
}