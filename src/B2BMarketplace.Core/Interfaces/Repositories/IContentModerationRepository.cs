using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.DTOs;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for ContentReport entity operations
    /// </summary>
    public interface IContentModerationRepository
    {
        /// <summary>
        /// Create a new content report
        /// </summary>
        /// <param name="report">ContentReport entity to create</param>
        /// <returns>Created ContentReport entity</returns>
        Task<ContentReport> CreateReportAsync(ContentReport report);

        /// <summary>
        /// Get content reports with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter criteria</param>
        /// <returns>Collection of ContentReport entities</returns>
        Task<IEnumerable<ContentReport>> GetReportsAsync(ReportFilterDto filter);

        /// <summary>
        /// Get content report by ID
        /// </summary>
        /// <param name="id">ContentReport ID</param>
        /// <returns>ContentReport entity or null if not found</returns>
        Task<ContentReport?> GetReportByIdAsync(Guid id);

        /// <summary>
        /// Update an existing content report
        /// </summary>
        /// <param name="report">ContentReport entity with updated values</param>
        /// <returns>Updated ContentReport entity</returns>
        Task<ContentReport> UpdateReportAsync(ContentReport report);

        /// <summary>
        /// Get repeat offenders (users with multiple resolved reports)
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Collection of RepeatOffenderDto</returns>
        Task<IEnumerable<RepeatOffenderDto>> GetRepeatOffendersAsync(int page, int pageSize);

        /// <summary>
        /// Get reports by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Collection of ContentReport entities</returns>
        Task<IEnumerable<ContentReport>> GetReportsByUserAsync(Guid userId);

        /// <summary>
        /// Get count of reports by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Count of reports</returns>
        Task<int> GetReportCountByUserAsync(Guid userId);
    }
}