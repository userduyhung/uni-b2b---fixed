using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for audit log operations (Admin only)
    /// </summary>
    [ApiController]
    [Route("api/admin/audit-logs")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        /// <summary>
        /// Constructor for AuditLogController
        /// </summary>
        /// <param name="auditLogService">Audit log service</param>
        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Gets audit logs with optional filtering and pagination (Admin only)
        /// </summary>
        /// <param name="action">Filter by action type</param>
        /// <param name="entityName">Filter by entity name</param>
        /// <param name="userId">Filter by affected user ID</param>
        /// <param name="operationType">Filter by operation type</param>
        /// <param name="startDate">Filter by start date</param>
        /// <param name="endDate">Filter by end date</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 50, max: 100)</param>
        /// <returns>Paged list of audit logs</returns>
        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] string? action = null,
            [FromQuery] string? entityName = null,
            [FromQuery] Guid? userId = null,
            [FromQuery] string? operationType = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                Console.WriteLine($"GetAuditLogs called with parameters: action={action}, entityName={entityName}, userId={userId}, operationType={operationType}, page={page}, pageSize={pageSize}");

                // Validate parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 50;
                if (pageSize > 100) pageSize = 100;

                Console.WriteLine("About to call _auditLogService.GetAllLogsAsync");

                // Get audit logs with all filters applied
                var allLogs = await _auditLogService.GetAllLogsAsync(action, entityName, userId, operationType, startDate, endDate);

                Console.WriteLine($"_auditLogService.GetAllLogsAsync returned: {allLogs?.Count ?? 0} logs");

                // Handle null result
                var logs = allLogs ?? new List<B2BMarketplace.Core.Entities.AuditLog>();
                
                // Apply pagination
                var totalItems = logs.Count;
                var pagedLogs = logs
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                Console.WriteLine($"After pagination: {pagedLogs.Count} logs");

                // Convert audit logs to dictionaries to ensure proper serialization
                var auditLogDtos = pagedLogs.Select(log => new
                {
                    Id = log.Id,
                    Action = log.Action,
                    EntityName = log.EntityName,
                    EntityId = log.EntityId,
                    UserId = log.UserId,
                    UserName = log.UserName,
                    UserRole = log.UserRole,
                    Details = log.Details,
                    Timestamp = log.Timestamp,
                    IPAddress = log.IPAddress,
                    UserAgent = log.UserAgent,
                    OperationType = log.OperationType,
                    CreatedAt = log.CreatedAt,
                    CreatedBy = log.CreatedBy,
                    CreatedAtCheck = log.CreatedAtCheck
                }).ToList();

                var result = new
                {
                    message = "Audit logs retrieved successfully",
                    data = new
                    {
                        items = auditLogDtos,
                        currentPage = page,
                        pageSize = pageSize,
                        totalItems = totalItems,
                        totalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                        hasPreviousPage = page > 1,
                        hasNextPage = page < Math.Ceiling((double)totalItems / pageSize)
                    },
                    timestamp = DateTime.UtcNow
                };

                Console.WriteLine("Returning successful result");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAuditLogs: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nStack Trace: {ex.StackTrace}");
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving audit logs",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Gets audit logs by user ID (Admin only)
        /// </summary>
        /// <param name="userId">ID of the user to get audit logs for</param>
        /// <returns>List of audit logs for the specified user</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAuditLogsByUser(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        error = "User ID is required",
                        timestamp = DateTime.UtcNow
                    });
                }

                var auditLogs = await _auditLogService.GetLogsByUserAsync(userId);

                // Handle null result
                var logs = auditLogs ?? new List<B2BMarketplace.Core.Entities.AuditLog>();

                // Convert audit logs to dictionaries to ensure proper serialization
                var auditLogDtos = logs.Select(log => new
                {
                    Id = log.Id,
                    Action = log.Action,
                    EntityName = log.EntityName,
                    EntityId = log.EntityId,
                    UserId = log.UserId,
                    UserName = log.UserName,
                    UserRole = log.UserRole,
                    Details = log.Details,
                    Timestamp = log.Timestamp,
                    IPAddress = log.IPAddress,
                    UserAgent = log.UserAgent,
                    OperationType = log.OperationType,
                    CreatedAt = log.CreatedAt,
                    CreatedBy = log.CreatedBy,
                    CreatedAtCheck = log.CreatedAtCheck
                }).ToList();

                return Ok(new
                {
                    message = "Audit logs retrieved successfully",
                    data = auditLogDtos,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAuditLogsByUser: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nStack Trace: {ex.StackTrace}");
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving audit logs for user",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }
    }
}
