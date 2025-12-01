using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface for certification data access operations
    /// </summary>
    public interface ICertificationRepository
    {
        /// <summary>
        /// Gets a certification by ID
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <returns>Certification entity if found, null otherwise</returns>
        Task<Certification?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets a certification by ID with seller profile included
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <returns>Certification entity if found, null otherwise</returns>
        Task<Certification?> GetByIdWithSellerProfileAsync(Guid id);

        /// <summary>
        /// Gets all certifications for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>List of certifications</returns>
        Task<List<Certification>> GetBySellerProfileIdAsync(Guid sellerProfileId);

        /// <summary>
        /// Gets certifications by status
        /// </summary>
        /// <param name="status">Certification status to filter by</param>
        /// <returns>List of certifications with the specified status</returns>
        Task<List<Certification>> GetByStatusAsync(CertificationStatus status);

        /// <summary>
        /// Creates a new certification
        /// </summary>
        /// <param name="certification">Certification entity to create</param>
        /// <returns>Created certification entity</returns>
        Task<Certification> CreateAsync(Certification certification);

        /// <summary>
        /// Updates an existing certification
        /// </summary>
        /// <param name="certification">Certification entity to update</param>
        /// <returns>Updated certification entity</returns>
        Task<Certification> UpdateAsync(Certification certification);
    }
}