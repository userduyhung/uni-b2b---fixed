using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Services
{
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
            Guid? userId,
            string? userName,
            string? userRole,
            string? details,
            string operationType,
            string? ipAddress = null,
            string? userAgent = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Action = action ?? string.Empty,
                    EntityName = entityName ?? string.Empty,
                    EntityId = entityId,
                    UserId = userId,
                    UserName = userName,
                    UserRole = userRole,
                    Details = details,
                    OperationType = operationType ?? string.Empty,
                    IPAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedAtCheck = DateTime.UtcNow,
                    CreatedBy = userId?.ToString()
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in LogActionAsync: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nStack Trace: {ex.StackTrace}");
                // Log the error but don't throw it to prevent cascading failures
                // Audit logging should not break the main flow
            }
        }

        public async Task<List<AuditLog>> GetLogsByEntityAsync(string entityName, int entityId)
        {
            try
            {
                return await _context.AuditLogs
                    .Where(log => log.EntityName == entityName && log.EntityId == entityId)
                    .OrderByDescending(log => log.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetLogsByEntityAsync: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nStack Trace: {ex.StackTrace}");
                // Return an empty list in case of error to prevent 500 errors
                return new List<AuditLog>();
            }
        }

        public async Task<List<AuditLog>> GetLogsByUserAsync(Guid userId)
        {
            try
            {
                return await _context.AuditLogs
                    .Where(log => log.UserId == userId)
                    .OrderByDescending(log => log.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetLogsByUserAsync: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nStack Trace: {ex.StackTrace}");
                // Return an empty list in case of error to prevent 500 errors
                return new List<AuditLog>();
            }
        }

        public async Task<List<AuditLog>> GetLogsByActionAsync(string action)
        {
            try
            {
                return await _context.AuditLogs
                    .Where(log => log.Action == action)
                    .OrderByDescending(log => log.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetLogsByActionAsync: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nStack Trace: {ex.StackTrace}");
                // Return an empty list in case of error to prevent 500 errors
                return new List<AuditLog>();
            }
        }

        public async Task<List<AuditLog>> GetLogsByOperationTypeAsync(string operationType)
        {
            try
            {
                return await _context.AuditLogs
                    .Where(log => log.OperationType == operationType)
                    .OrderByDescending(log => log.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetLogsByOperationTypeAsync: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nStack Trace: {ex.StackTrace}");
                // Return an empty list in case of error to prevent 500 errors
                return new List<AuditLog>();
            }
        }

        public async Task<List<AuditLog>> GetAllLogsAsync(string? action = null, string? entityName = null, Guid? userId = null, string? operationType = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                Console.WriteLine("=== GetAllLogsAsync START ===");
                Console.WriteLine("GetAllLogsAsync called with parameters:");
                Console.WriteLine($"  action: {action}");
                Console.WriteLine($"  entityName: {entityName}");
                Console.WriteLine($"  userId: {userId}");
                Console.WriteLine($"  operationType: {operationType}");
                Console.WriteLine($"  startDate: {startDate}");
                Console.WriteLine($"  endDate: {endDate}");

                // Ensure database is properly initialized
                if (_context == null)
                {
                    Console.WriteLine("ERROR: _context is null!");
                    throw new InvalidOperationException("Database context is null");
                }

                if (_context.AuditLogs == null)
                {
                    Console.WriteLine("ERROR: _context.AuditLogs is null!");
                    throw new InvalidOperationException("AuditLogs DbSet is not initialized");
                }

                Console.WriteLine("Context check passed, proceeding with query...");

                // Force database connectivity test
                try
                {
                    var dbTest = await _context.AuditLogs.AnyAsync();
                    Console.WriteLine($"Database connectivity test: {dbTest}");
                }
                catch (Exception dbEx)
                {
                    Console.WriteLine($"ERROR: Database connectivity failed: {dbEx.Message}");
                    throw new InvalidOperationException($"Database connectivity failed: {dbEx.Message}", dbEx);
                }

                var query = _context.AuditLogs.AsQueryable();
                Console.WriteLine("Successfully created query");

                if (!string.IsNullOrEmpty(action))
                {
                    query = query.Where(log => log.Action.Contains(action));
                    Console.WriteLine($"Applied action filter: {action}");
                }

                if (!string.IsNullOrEmpty(entityName))
                {
                    query = query.Where(log => log.EntityName == entityName);
                    Console.WriteLine($"Applied entityName filter: {entityName}");
                }

                if (userId.HasValue)
                {
                    query = query.Where(log => log.UserId == userId);
                    Console.WriteLine($"Applied userId filter: {userId}");
                }

                if (!string.IsNullOrEmpty(operationType))
                {
                    query = query.Where(log => log.OperationType == operationType);
                    Console.WriteLine($"Applied operationType filter: {operationType}");
                }

                if (startDate.HasValue)
                {
                    query = query.Where(log => log.Timestamp >= startDate.Value);
                    Console.WriteLine($"Applied startDate filter: {startDate}");
                }

                if (endDate.HasValue)
                {
                    query = query.Where(log => log.Timestamp <= endDate.Value);
                    Console.WriteLine($"Applied endDate filter: {endDate}");
                }

                Console.WriteLine("About to execute query and call ToListAsync()");
                var result = await query
                    .OrderByDescending(log => log.Timestamp)
                    .ToListAsync();
                Console.WriteLine($"Query executed successfully, returned {result.Count} records");
                Console.WriteLine("=== GetAllLogsAsync SUCCESS ===");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== GetAllLogsAsync ERROR ===");
                Console.WriteLine($"Exception in GetAllLogsAsync: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Exception Type: {ex.GetType().FullName}");
                
                // Check if it's a database-related exception
                if (ex.Message.Contains("database") || ex.Message.Contains("table") || ex.Message.Contains("AuditLog"))
                {
                    Console.WriteLine("This appears to be a database-related exception");
                }
                
                // Re-throw the exception to get proper error details
                throw;
            }
        }

        public async Task<int> CleanupExpiredLogsAsync(int retentionYears = 7)
        {
            var cutoffDate = DateTime.UtcNow.AddYears(-retentionYears);

            var expiredLogs = await _context.AuditLogs
                .Where(log => log.Timestamp < cutoffDate)
                .ToListAsync();

            _context.AuditLogs.RemoveRange(expiredLogs);

            var deletedCount = expiredLogs.Count;
            await _context.SaveChangesAsync();

            return deletedCount;
        }
    }
}