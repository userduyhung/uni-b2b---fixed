using B2BPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace B2BPlatform.Services
{
    public interface IAuditLogService
    {
        Task LogActionAsync(string action, string entityName, int entityId, string? userId, string? userName, string? userRole, string? details, string operationType, string? ipAddress = null, string? userAgent = null);
        Task<List<AuditLog>> GetLogsByEntityAsync(string entityName, int entityId);
        Task<List<AuditLog>> GetLogsByUserAsync(string userId);
        Task<List<AuditLog>> GetLogsByActionAsync(string action);
        Task<List<AuditLog>> GetLogsByOperationTypeAsync(string operationType);
    }
    
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;
        
        public AuditLogService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task LogActionAsync(
            string action, 
            string entityName, 
            int entityId, 
            string? userId, 
            string? userName, 
            string? userRole, 
            string? details, 
            string operationType, 
            string? ipAddress = null, 
            string? userAgent = null)
        {
            var auditLog = new AuditLog
            {
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                UserId = userId,
                UserName = userName,
                UserRole = userRole,
                Details = details,
                OperationType = operationType,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow,
                CreatedBy = userId
            };
            
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        
        public async Task<List<AuditLog>> GetLogsByEntityAsync(string entityName, int entityId)
        {
            return await _context.AuditLogs
                .Where(log => log.EntityName == entityName && log.EntityId == entityId)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }
        
        public async Task<List<AuditLog>> GetLogsByUserAsync(string userId)
        {
            return await _context.AuditLogs
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }
        
        public async Task<List<AuditLog>> GetLogsByActionAsync(string action)
        {
            return await _context.AuditLogs
                .Where(log => log.Action == action)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }
        
        public async Task<List<AuditLog>> GetLogsByOperationTypeAsync(string operationType)
        {
            return await _context.AuditLogs
                .Where(log => log.OperationType == operationType)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }
    }
}