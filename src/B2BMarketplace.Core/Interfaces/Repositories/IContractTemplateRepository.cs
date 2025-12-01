using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for ContractTemplate entities
    /// </summary>
    public interface IContractTemplateRepository
    {
        /// <summary>
        /// Creates a new contract template in the database
        /// </summary>
        /// <param name="contractTemplate">Contract template entity to create</param>
        /// <returns>Created contract template entity</returns>
        Task<ContractTemplate> CreateAsync(ContractTemplate contractTemplate);

        /// <summary>
        /// Updates an existing contract template in the database
        /// </summary>
        /// <param name="contractTemplate">Contract template entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateAsync(ContractTemplate contractTemplate);

        /// <summary>
        /// Deletes a contract template by ID
        /// </summary>
        /// <param name="id">Contract template ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Gets a contract template by its ID
        /// </summary>
        /// <param name="id">Contract template ID to search for</param>
        /// <returns>Contract template entity if found, null otherwise</returns>
        Task<ContractTemplate?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all contract templates with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result containing contract templates</returns>
        Task<PagedResult<ContractTemplate>> GetAllAsync(int page, int pageSize);

        /// <summary>
        /// Gets active contract templates by seller profile ID
        /// </summary>
        /// <param name="sellerProfileId">The seller profile ID</param>
        /// <returns>Collection of active contract templates</returns>
        Task<IEnumerable<ContractTemplate>> GetActiveBySellerProfileIdAsync(Guid sellerProfileId);

        /// <summary>
        /// Gets a contract template by its ID with navigation properties
        /// </summary>
        /// <param name="id">The contract template ID</param>
        /// <returns>The contract template with navigation properties</returns>
        Task<ContractTemplate?> GetByIdWithNavigationAsync(Guid id);
    }
}