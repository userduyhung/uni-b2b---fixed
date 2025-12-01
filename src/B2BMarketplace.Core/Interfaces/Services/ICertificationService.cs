using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for certification management operations
    /// </summary>
    public interface ICertificationService
    {
        /// <summary>
        /// Creates a new certification for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="createDto">Certification creation data</param>
        /// <returns>Created certification DTO</returns>
        Task<CertificationDto> CreateCertificationAsync(Guid sellerProfileId, CreateCertificationDto createDto);

        /// <summary>
        /// Gets all certifications for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>List of certification DTOs</returns>
        Task<List<CertificationDto>> GetCertificationsBySellerAsync(Guid sellerProfileId);

        /// <summary>
        /// Gets all certifications with a specific status (for admin use)
        /// </summary>
        /// <param name="status">Certification status to filter by</param>
        /// <returns>List of certification DTOs</returns>
        Task<List<CertificationDto>> GetCertificationsByStatusAsync(CertificationStatus status);

        /// <summary>
        /// Updates the status of a certification (for admin use)
        /// </summary>
        /// <param name="certificationId">Certification ID</param>
        /// <param name="updateDto">Status update data</param>
        /// <returns>Updated certification DTO</returns>
        Task<CertificationDto> UpdateCertificationStatusAsync(Guid certificationId, UpdateCertificationStatusDto updateDto);

        /// <summary>
        /// Gets a certification by ID
        /// </summary>
        /// <param name="certificationId">Certification ID</param>
        /// <returns>Certification DTO</returns>
        Task<CertificationDto?> GetCertificationByIdAsync(Guid certificationId);
    }
}