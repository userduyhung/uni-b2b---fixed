using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for ModerationAuditLog entity operations
    /// </summary>
    public class ModerationAuditLogRepository : IModerationAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Database context</param>
        public ModerationAuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create a new moderation audit log entry
        /// </summary>
        /// <param name="auditLog">ModerationAuditLog entity to create</param>
        /// <returns>Created ModerationAuditLog entity</returns>
        public async Task<ModerationAuditLog> CreateAsync(ModerationAuditLog auditLog)
        {
            _context.ModerationAuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return auditLog;
        }

        /// <summary>
        /// Get moderation audit logs by report ID
        /// </summary>
        /// <param name="reportId">Content report ID</param>
        /// <returns>Collection of ModerationAuditLog entities</returns>
        public async Task<IEnumerable<ModerationAuditLog>> GetByReportIdAsync(Guid reportId)
        {
            return await _context.ModerationAuditLogs
                .Where(mal => mal.ContentReportId == reportId)
                .ToListAsync();
        }

        /// <summary>
        /// Get moderation audit logs by moderator ID
        /// </summary>
        /// <param name="moderatorId">Moderator user ID</param>
        /// <returns>Collection of ModerationAuditLog entities</returns>
        public async Task<IEnumerable<ModerationAuditLog>> GetByModeratorIdAsync(Guid moderatorId)
        {
            return await _context.ModerationAuditLogs
                .Where(mal => mal.ModeratedById == moderatorId)
                .ToListAsync();
        }
    }
}