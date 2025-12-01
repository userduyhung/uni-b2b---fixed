using B2BMarketplace.Core.DTOs;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for Content Moderation operations
    /// </summary>
    public interface IContentModerationService
    {
        /// <summary>
        /// Create a new content report
        /// </summary>
        /// <param name="reportDto">Content report creation DTO</param>
        /// <param name="reportedById">ID of user who reported the content</param>
        /// <returns>Created ContentReport DTO</returns>
        Task<ContentReportDto> CreateReportAsync(CreateReportDto reportDto, Guid reportedById);

        /// <summary>
        /// Get content reports with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter criteria</param>
        /// <returns>Collection of ContentReport DTOs</returns>
        Task<IEnumerable<ContentReportDto>> GetReportsAsync(ReportFilterDto filter);

        /// <summary>
        /// Get content report by ID
        /// </summary>
        /// <param name="id">ContentReport ID</param>
        /// <returns>ContentReport DTO or null if not found</returns>
        Task<ContentReportDto?> GetReportByIdAsync(Guid id);

        /// <summary>
        /// Resolve a content report
        /// </summary>
        /// <param name="id">ContentReport ID</param>
        /// <param name="resolution">Resolution DTO</param>
        /// <param name="resolvedById">ID of admin who resolved the report</param>
        /// <returns>Resolved ContentReport DTO</returns>
        Task<ContentReportDto> ResolveReportAsync(Guid id, ModerationResolutionDto resolution, Guid resolvedById);

        /// <summary>
        /// Get repeat offenders (users with multiple resolved reports)
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Collection of RepeatOffender DTOs</returns>
        Task<IEnumerable<RepeatOffenderDto>> GetRepeatOffendersAsync(int page, int pageSize);

        /// <summary>
        /// Notify the user who reported content about the resolution
        /// </summary>
        /// <param name="reportId">ContentReport ID</param>
        /// <returns>Task</returns>
        Task NotifyReporterAsync(Guid reportId);
    }
}