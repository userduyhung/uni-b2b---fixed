using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for contract template operations
    /// </summary>
    public interface IContractTemplateService
    {
        /// <summary>
        /// Creates a new contract template
        /// </summary>
        /// <param name="contractTemplate">Contract template to create</param>
        /// <returns>Created contract template</returns>
        Task<ContractTemplate> CreateTemplateAsync(ContractTemplate contractTemplate);

        /// <summary>
        /// Updates an existing contract template
        /// </summary>
        /// <param name="contractTemplate">Contract template with updated values</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateTemplateAsync(ContractTemplate contractTemplate);

        /// <summary>
        /// Gets a contract template by its ID
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <returns>Contract template if found, null otherwise</returns>
        Task<ContractTemplate?> GetTemplateByIdAsync(Guid id);

        /// <summary>
        /// Gets all contract templates for a seller with pagination
        /// </summary>
        /// <param name="sellerProfileId">ID of the seller</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result of contract templates</returns>
        Task<PagedResult<ContractTemplate>> GetTemplatesBySellerAsync(Guid sellerProfileId, int page, int pageSize);

        /// <summary>
        /// Generates a contract instance from a template
        /// </summary>
        /// <param name="templateId">ID of the template to use</param>
        /// <param name="buyerProfileId">ID of the buyer</param>
        /// <param name="sellerProfileId">ID of the seller</param>
        /// <param name="rfqId">Optional RFQ ID associated with the contract</param>
        /// <param name="quoteId">Optional Quote ID associated with the contract</param>
        /// <returns>Generated contract instance</returns>
        Task<ContractInstance> GenerateContractInstanceAsync(Guid templateId, Guid buyerProfileId, Guid sellerProfileId, Guid? rfqId = null, Guid? quoteId = null);
    }
}