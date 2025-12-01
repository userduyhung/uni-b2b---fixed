using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for ContractInstance entities
    /// </summary>
    public interface IContractInstanceRepository
    {
        /// <summary>
        /// Creates a new contract instance in the database
        /// </summary>
        /// <param name="contractInstance">Contract instance entity to create</param>
        /// <returns>Created contract instance entity</returns>
        Task<ContractInstance> CreateAsync(ContractInstance contractInstance);

        /// <summary>
        /// Updates an existing contract instance in the database
        /// </summary>
        /// <param name="contractInstance">Contract instance entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateAsync(ContractInstance contractInstance);

        /// <summary>
        /// Deletes a contract instance by ID
        /// </summary>
        /// <param name="id">Contract instance ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Gets a contract instance by its ID
        /// </summary>
        /// <param name="id">Contract instance ID to search for</param>
        /// <returns>Contract instance entity if found, null otherwise</returns>
        Task<ContractInstance?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all contract instances with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result containing contract instances</returns>
        Task<PagedResult<ContractInstance>> GetAllAsync(int page, int pageSize);

        /// <summary>
        /// Gets contract instances by buyer profile ID
        /// </summary>
        /// <param name="buyerProfileId">The buyer profile ID</param>
        /// <returns>Collection of contract instances</returns>
        Task<IEnumerable<ContractInstance>> GetByBuyerProfileIdAsync(Guid buyerProfileId);

        /// <summary>
        /// Gets contract instances by seller profile ID
        /// </summary>
        /// <param name="sellerProfileId">The seller profile ID</param>
        /// <returns>Collection of contract instances</returns>
        Task<IEnumerable<ContractInstance>> GetBySellerProfileIdAsync(Guid sellerProfileId);

        /// <summary>
        /// Gets a contract instance by its number
        /// </summary>
        /// <param name="contractNumber">The contract number</param>
        /// <returns>The contract instance or null</returns>
        Task<ContractInstance?> GetByNumberAsync(string contractNumber);
    }
}