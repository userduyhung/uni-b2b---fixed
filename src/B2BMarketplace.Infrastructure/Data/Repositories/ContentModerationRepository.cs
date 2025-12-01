using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using B2BMarketplace.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for ContentReport entity operations
    /// </summary>
    public class ContentModerationRepository : IContentModerationRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Database context</param>
        public ContentModerationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create a new content report
        /// </summary>
        /// <param name="report">ContentReport entity to create</param>
        /// <returns>Created ContentReport entity</returns>
        public async Task<ContentReport> CreateReportAsync(ContentReport report)
        {
            report.ReportedAt = DateTime.UtcNow;
            report.Status = Core.Enums.ReportStatus.Pending;

            _context.ContentReports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        /// <summary>
        /// Get content reports with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter criteria</param>
        /// <returns>Collection of ContentReport entities</returns>
        public async Task<IEnumerable<ContentReport>> GetReportsAsync(ReportFilterDto filter)
        {
            var query = _context.ContentReports
                .Include(cr => cr.ReportedBy)
                .AsQueryable();

            // Apply filters
            if (filter.Status.HasValue)
            {
                query = query.Where(cr => cr.Status == filter.Status.Value);
            }

            if (filter.ContentType.HasValue)
            {
                query = query.Where(cr => cr.ContentType == filter.ContentType.Value);
            }

            // Apply pagination
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Get content report by ID
        /// </summary>
        /// <param name="id">ContentReport ID</param>
        /// <returns>ContentReport entity or null if not found</returns>
        public async Task<ContentReport?> GetReportByIdAsync(Guid id)
        {
            return await _context.ContentReports
                .Include(cr => cr.ReportedBy)
                .Include(cr => cr.ResolvedBy)
                .FirstOrDefaultAsync(cr => cr.Id == id);
        }

        /// <summary>
        /// Update an existing content report
        /// </summary>
        /// <param name="report">ContentReport entity with updated values</param>
        /// <returns>Updated ContentReport entity</returns>
        public async Task<ContentReport> UpdateReportAsync(ContentReport report)
        {
            if (report.Status == Core.Enums.ReportStatus.Resolved)
            {
                report.ResolvedAt = DateTime.UtcNow;
            }

            _context.ContentReports.Update(report);
            await _context.SaveChangesAsync();

            return report;
        }

        /// <summary>
        /// Get repeat offenders (users with multiple resolved reports)
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Collection of RepeatOffenderDto</returns>
        public async Task<IEnumerable<RepeatOffenderDto>> GetRepeatOffendersAsync(int page, int pageSize)
        {
            var offenders = await _context.ContentReports
                .Where(cr => cr.Status == Core.Enums.ReportStatus.Resolved)
                .GroupBy(cr => cr.ReportedById)
                .Select(g => new RepeatOffenderDto
                {
                    UserId = g.Key,
                    Email = g.First().ReportedBy.Email,
                    ReportCount = g.Count(),
                    LastReportedAt = g.Max(cr => cr.ReportedAt)
                })
                .OrderByDescending(ro => ro.ReportCount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return offenders;
        }

        /// <summary>
        /// Get reports by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Collection of ContentReport entities</returns>
        public async Task<IEnumerable<ContentReport>> GetReportsByUserAsync(Guid userId)
        {
            return await _context.ContentReports
                .Where(cr => cr.ReportedById == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Get count of reports by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Count of reports</returns>
        public async Task<int> GetReportCountByUserAsync(Guid userId)
        {
            return await _context.ContentReports
                .CountAsync(cr => cr.ReportedById == userId && cr.Status == Core.Enums.ReportStatus.Resolved);
        }
    }
}