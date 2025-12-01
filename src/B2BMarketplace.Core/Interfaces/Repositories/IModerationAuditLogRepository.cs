using B2BMarketplace.Core.Entities;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for ModerationAuditLog entity operations
    /// </summary>
    public interface IModerationAuditLogRepository
    {
        /// <summary>
        /// Create a new moderation audit log entry
        /// </summary>
        /// <param name="auditLog">ModerationAuditLog entity to create</param>
        /// <returns>Created ModerationAuditLog entity</returns>
        Task<ModerationAuditLog> CreateAsync(ModerationAuditLog auditLog);

        /// <summary>
        /// Get moderation audit logs by report ID
        /// </summary>
        /// <param name="reportId">Content report ID</param>
        /// <returns>Collection of ModerationAuditLog entities</returns>
        Task<IEnumerable<ModerationAuditLog>> GetByReportIdAsync(Guid reportId);

        /// <summary>
        /// Get moderation audit logs by moderator ID
        /// </summary>
        /// <param name="moderatorId">Moderator user ID</param>
        /// <returns>Collection of ModerationAuditLog entities</returns>
        Task<IEnumerable<ModerationAuditLog>> GetByModeratorIdAsync(Guid moderatorId);
    }
}