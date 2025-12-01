using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BMarketplace.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for Content Moderation operations
    /// </summary>
    public class ContentModerationService : IContentModerationService
    {
        private readonly IContentModerationRepository _contentModerationRepository;
        private readonly IModerationAuditLogRepository _moderationAuditLogRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contentModerationRepository">Content moderation repository</param>
        /// <param name="moderationAuditLogRepository">Moderation audit log repository</param>
        /// <param name="notificationService">Notification service</param>
        /// <param name="userRepository">User repository</param>
        /// <param name="productRepository">Product repository</param>
        public ContentModerationService(
            IContentModerationRepository contentModerationRepository,
            IModerationAuditLogRepository moderationAuditLogRepository,
            INotificationService notificationService,
            IUserRepository userRepository,
            IProductRepository productRepository)
        {
            _contentModerationRepository = contentModerationRepository;
            _moderationAuditLogRepository = moderationAuditLogRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Create a new content report
        /// </summary>
        /// <param name="reportDto">Content report creation DTO</param>
        /// <param name="reportedById">ID of user who reported the content</param>
        /// <returns>Created ContentReport DTO</returns>
        public async Task<ContentReportDto> CreateReportAsync(CreateReportDto reportDto, Guid reportedById)
        {
            // Validate that user exists
            var user = await _userRepository.GetUserByIdAsync(reportedById);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(reportedById));
            }

            // Validate that content exists (for products)
            if (reportDto.ContentType == Core.Enums.ContentType.Product)
            {
                var product = await _productRepository.GetByIdAsync(reportDto.ReportedContentId);
                if (product == null)
                {
                    throw new ArgumentException("Product not found", nameof(reportDto.ReportedContentId));
                }
            }

            // Create content report entity
            var report = new ContentReport
            {
                ReportedContentId = reportDto.ReportedContentId,
                ContentType = reportDto.ContentType,
                ReportedById = reportedById,
                Reason = reportDto.Reason,
                Description = reportDto.Description
            };

            // Save to database
            var createdReport = await _contentModerationRepository.CreateReportAsync(report);

            // Return DTO
            return MapToDto(createdReport);
        }

        /// <summary>
        /// Get content reports with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter criteria</param>
        /// <returns>Collection of ContentReport DTOs</returns>
        public async Task<IEnumerable<ContentReportDto>> GetReportsAsync(ReportFilterDto filter)
        {
            var reports = await _contentModerationRepository.GetReportsAsync(filter);
            return reports.Select(MapToDto);
        }

        /// <summary>
        /// Get content report by ID
        /// </summary>
        /// <param name="id">ContentReport ID</param>
        /// <returns>ContentReport DTO or null if not found</returns>
        public async Task<ContentReportDto?> GetReportByIdAsync(Guid id)
        {
            var report = await _contentModerationRepository.GetReportByIdAsync(id);
            return report == null ? null : MapToDto(report);
        }

        /// <summary>
        /// Resolve a content report
        /// </summary>
        /// <param name="id">ContentReport ID</param>
        /// <param name="resolution">Resolution DTO</param>
        /// <param name="resolvedById">ID of admin who resolved the report</param>
        /// <returns>Resolved ContentReport DTO</returns>
        public async Task<ContentReportDto> ResolveReportAsync(Guid id, ModerationResolutionDto resolution, Guid resolvedById)
        {
            // Get existing report
            var report = await _contentModerationRepository.GetReportByIdAsync(id);
            if (report == null)
            {
                throw new ArgumentException("Content report not found", nameof(id));
            }

            // Validate that admin exists
            var admin = await _userRepository.GetUserByIdAsync(resolvedById);
            if (admin == null)
            {
                throw new ArgumentException("Admin user not found", nameof(resolvedById));
            }

            // Update report properties
            report.Status = Core.Enums.ReportStatus.Resolved;
            report.ResolvedById = resolvedById;
            report.ResolvedAt = DateTime.UtcNow;
            report.ActionTaken = resolution.Action;

            // For hide action, we need to hide the content
            if (resolution.Action == Core.Enums.ModerationAction.Hide)
            {
                await HideContentAsync(report);
            }
            // For remove action, we need to remove the content
            else if (resolution.Action == Core.Enums.ModerationAction.Remove)
            {
                await RemoveContentAsync(report);
            }

            // Save audit log
            var auditLog = new ModerationAuditLog
            {
                ContentReportId = report.Id,
                ModeratedById = resolvedById,
                Action = resolution.Action,
                Notes = resolution.Notes
            };

            await _moderationAuditLogRepository.CreateAsync(auditLog);

            // Save to database
            var updatedReport = await _contentModerationRepository.UpdateReportAsync(report);

            // Return DTO
            return MapToDto(updatedReport);
        }

        /// <summary>
        /// Get repeat offenders (users with multiple resolved reports)
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Collection of RepeatOffender DTOs</returns>
        public async Task<IEnumerable<RepeatOffenderDto>> GetRepeatOffendersAsync(int page, int pageSize)
        {
            return await _contentModerationRepository.GetRepeatOffendersAsync(page, pageSize);
        }

        /// <summary>
        /// Notify the user who reported content about the resolution
        /// </summary>
        /// <param name="reportId">ContentReport ID</param>
        /// <returns>Task</returns>
        public async Task NotifyReporterAsync(Guid reportId)
        {
            var report = await _contentModerationRepository.GetReportByIdAsync(reportId);
            if (report == null)
            {
                throw new ArgumentException("Content report not found", nameof(reportId));
            }

            var message = $"Your report about {report.ContentType} has been reviewed. " +
                         $"Action taken: {report.ActionTaken}. " +
                         $"Thank you for helping maintain the quality of our platform.";

            await _notificationService.CreateNotificationAsync(
                report.ReportedById,
                B2BMarketplace.Core.DTOs.NotificationType.RFQClosed, // Using existing notification type
                "Content Report Resolved",
                message);
        }

        /// <summary>
        /// Hide content (mark as inactive)
        /// </summary>
        /// <param name="report">Content report</param>
        private async Task HideContentAsync(ContentReport report)
        {
            // For now, we only support hiding products
            if (report.ContentType == Core.Enums.ContentType.Product)
            {
                var product = await _productRepository.GetByIdAsync(report.ReportedContentId);
                if (product != null)
                {
                    product.IsActive = false;
                    await _productRepository.UpdateAsync(product);
                }
            }
        }

        /// <summary>
        /// Remove content (delete permanently)
        /// </summary>
        /// <param name="report">Content report</param>
        private async Task RemoveContentAsync(ContentReport report)
        {
            // For now, we only support removing products
            if (report.ContentType == Core.Enums.ContentType.Product)
            {
                await _productRepository.DeleteAsync(report.ReportedContentId);
            }
        }

        /// <summary>
        /// Map ContentReport entity to ContentReport DTO
        /// </summary>
        /// <param name="report">ContentReport entity</param>
        /// <returns>ContentReport DTO</returns>
        private static ContentReportDto MapToDto(ContentReport report)
        {
            return new ContentReportDto
            {
                Id = report.Id,
                ReportedContentId = report.ReportedContentId,
                ContentType = report.ContentType,
                ReportedById = report.ReportedById,
                Reason = report.Reason,
                Description = report.Description,
                Status = report.Status,
                ResolvedById = report.ResolvedById,
                ReportedAt = report.ReportedAt,
                ResolvedAt = report.ResolvedAt,
                ActionTaken = report.ActionTaken
            };
        }
    }
}