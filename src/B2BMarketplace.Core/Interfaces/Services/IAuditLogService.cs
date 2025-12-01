using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface IAuditLogService
    {
        Task LogActionAsync(string action, string entityName, int entityId, Guid? userId, string? userName, string? userRole, string? details, string operationType, string? ipAddress = null, string? userAgent = null);
        Task<List<AuditLog>> GetLogsByEntityAsync(string entityName, int entityId);
        Task<List<AuditLog>> GetLogsByUserAsync(Guid userId);
        Task<List<AuditLog>> GetLogsByActionAsync(string action);
        Task<List<AuditLog>> GetLogsByOperationTypeAsync(string operationType);
        Task<List<AuditLog>> GetAllLogsAsync(string? action = null, string? entityName = null, Guid? userId = null, string? operationType = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<int> CleanupExpiredLogsAsync(int retentionYears = 7);
    }
}