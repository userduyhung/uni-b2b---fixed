using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for seller verification operations
    /// </summary>
    public interface IVerificationService
    {
        /// <summary>
        /// Gets pending verification requests with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 50)</param>
        /// <returns>Paged list of pending verification requests</returns>
        Task<PagedResultDto<PendingVerificationDto>> GetPendingVerificationsAsync(int page = 1, int pageSize = 10);

        /// <summary>
        /// Gets detailed information for a verification request
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <returns>Verification details DTO</returns>
        Task<VerificationDetailsDto?> GetVerificationDetailsAsync(Guid id);

        /// <summary>
        /// Approves a verification request
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <param name="adminNotes">Optional notes from admin</param>
        /// <returns>Success result</returns>
        Task<bool> ApproveVerificationAsync(Guid id, string? adminNotes = null);

        /// <summary>
        /// Rejects a verification request
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <param name="adminNotes">Required notes from admin</param>
        /// <returns>Success result</returns>
        Task<bool> RejectVerificationAsync(Guid id, string adminNotes);

        /// <summary>
        /// Updates verification status for a seller based on their certifications and premium status
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>True if verification status was updated, false otherwise</returns>
        Task<bool> UpdateVerificationStatusAsync(Guid sellerProfileId);

        /// <summary>
        /// Gets all sellers with their verification and premium status with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 50)</param>
        /// <param name="status">Filter by verification status (optional: verified, unverified, premium)</param>
        /// <returns>Paged list of sellers with verification and premium status</returns>
        Task<PagedResultDto<VerificationSummaryDto>> GetAllVerificationsAsync(int page = 1, int pageSize = 10, string? status = null);

        /// <summary>
        /// Manually updates verification status for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="isVerified">Whether the seller should be marked as verified</param>
        /// <param name="adminNotes">Optional admin notes for the change</param>
        /// <returns>True if verification status was updated, false otherwise</returns>
        Task<bool> ManualUpdateVerificationAsync(Guid sellerProfileId, bool isVerified, string? adminNotes = null);
    }
}