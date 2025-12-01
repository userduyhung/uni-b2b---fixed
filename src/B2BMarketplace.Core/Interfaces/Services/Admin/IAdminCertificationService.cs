using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services.Admin
{
    /// <summary>
    /// Service interface for admin certification management
    /// </summary>
    public interface IAdminCertificationService
    {
        /// <summary>
        /// Gets all certifications with pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Paginated list of certifications</returns>
        Task<PagedResultDto<CertificationDto>> GetCertificationsAsync(int page = 1, int size = 10);

        /// <summary>
        /// Gets a certification by ID
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <returns>Certification DTO or null if not found</returns>
        Task<CertificationDto?> GetCertificationByIdAsync(Guid id);

        /// <summary>
        /// Creates a new certification
        /// </summary>
        /// <param name="certificationDto">Certification to create</param>
        /// <returns>Created certification with ID</returns>
        Task<CertificationDto> CreateCertificationAsync(CertificationDto certificationDto);

        /// <summary>
        /// Updates an existing certification
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <param name="certificationDto">Updated certification data</param>
        /// <returns>Updated certification or null if not found</returns>
        Task<CertificationDto?> UpdateCertificationAsync(Guid id, CertificationDto certificationDto);

        /// <summary>
        /// Deletes a certification by ID
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> DeleteCertificationAsync(Guid id);

        /// <summary>
        /// Searches certifications by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Paginated list of matching certifications</returns>
        Task<PagedResultDto<CertificationDto>> SearchCertificationsAsync(string searchTerm, int page = 1, int size = 10);

        /// <summary>
        /// Approves a seller's certification
        /// </summary>
        /// <param name="certificationId">ID of the certification to approve</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> ApproveCertificationAsync(Guid certificationId, Guid adminId);

        /// <summary>
        /// Rejects a seller's certification
        /// </summary>
        /// <param name="certificationId">ID of the certification to reject</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <param name="rejectionReason">Reason for rejection</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> RejectCertificationAsync(Guid certificationId, Guid adminId, string? rejectionReason);
    }
}