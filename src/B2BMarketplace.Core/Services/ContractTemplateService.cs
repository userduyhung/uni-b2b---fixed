using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for contract template operations
    /// </summary>
    public class ContractTemplateService : IContractTemplateService
    {
        private readonly IContractTemplateRepository _contractTemplateRepository;
        private readonly IContractInstanceRepository _contractInstanceRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly IBuyerProfileRepository _buyerProfileRepository;

        /// <summary>
        /// Constructor for ContractTemplateService
        /// </summary>
        /// <param name="contractTemplateRepository">Contract template repository</param>
        /// <param name="contractInstanceRepository">Contract instance repository</param>
        /// <param name="sellerProfileRepository">Seller profile repository</param>
        /// <param name="buyerProfileRepository">Buyer profile repository</param>
        public ContractTemplateService(
            IContractTemplateRepository contractTemplateRepository,
            IContractInstanceRepository contractInstanceRepository,
            ISellerProfileRepository sellerProfileRepository,
            IBuyerProfileRepository buyerProfileRepository)
        {
            _contractTemplateRepository = contractTemplateRepository;
            _contractInstanceRepository = contractInstanceRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _buyerProfileRepository = buyerProfileRepository;
        }

        /// <summary>
        /// Creates a new contract template
        /// </summary>
        /// <param name="contractTemplate">Contract template to create</param>
        /// <returns>Created contract template</returns>
        public async Task<ContractTemplate> CreateTemplateAsync(ContractTemplate contractTemplate)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(contractTemplate.Name))
                throw new ArgumentException("Template name is required", nameof(contractTemplate.Name));

            if (string.IsNullOrWhiteSpace(contractTemplate.Content))
                throw new ArgumentException("Template content is required", nameof(contractTemplate.Content));

            // Verify the seller profile exists
            var seller = await _sellerProfileRepository.GetByIdAsync(contractTemplate.CreatedBySellerProfileId);
            if (seller == null)
                throw new ArgumentException("Invalid seller profile ID", nameof(contractTemplate.CreatedBySellerProfileId));

            // Create the template
            return await _contractTemplateRepository.CreateAsync(contractTemplate);
        }

        /// <summary>
        /// Updates an existing contract template
        /// </summary>
        /// <param name="contractTemplate">Contract template with updated values</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateTemplateAsync(ContractTemplate contractTemplate)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(contractTemplate.Name))
                throw new ArgumentException("Template name is required", nameof(contractTemplate.Name));

            if (string.IsNullOrWhiteSpace(contractTemplate.Content))
                throw new ArgumentException("Template content is required", nameof(contractTemplate.Content));

            // Verify the template exists
            var existingTemplate = await _contractTemplateRepository.GetByIdAsync(contractTemplate.Id);
            if (existingTemplate == null)
                return false;

            // Update the template
            return await _contractTemplateRepository.UpdateAsync(contractTemplate);
        }

        /// <summary>
        /// Gets a contract template by its ID
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <returns>Contract template if found, null otherwise</returns>
        public async Task<ContractTemplate?> GetTemplateByIdAsync(Guid id)
        {
            return await _contractTemplateRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Gets all contract templates for a seller with pagination
        /// </summary>
        /// <param name="sellerProfileId">ID of the seller</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result of contract templates</returns>
        public async Task<PagedResult<ContractTemplate>> GetTemplatesBySellerAsync(Guid sellerProfileId, int page, int pageSize)
        {
            return await _contractTemplateRepository.GetAllAsync(page, pageSize);
        }

        /// <summary>
        /// Generates a contract instance from a template
        /// </summary>
        /// <param name="templateId">ID of the template to use</param>
        /// <param name="buyerProfileId">ID of the buyer</param>
        /// <param name="sellerProfileId">ID of the seller</param>
        /// <param name="rfqId">Optional RFQ ID associated with the contract</param>
        /// <param name="quoteId">Optional Quote ID associated with the contract</param>
        /// <returns>Generated contract instance</returns>
        public async Task<ContractInstance> GenerateContractInstanceAsync(Guid templateId, Guid buyerProfileId, Guid sellerProfileId, Guid? rfqId = null, Guid? quoteId = null)
        {
            // Verify the template exists
            var template = await _contractTemplateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new ArgumentException("Invalid contract template ID", nameof(templateId));

            // Verify buyer and seller profiles exist
            var buyer = await _buyerProfileRepository.GetByIdAsync(buyerProfileId);
            if (buyer == null)
                throw new ArgumentException("Invalid buyer profile ID", nameof(buyerProfileId));

            var seller = await _sellerProfileRepository.GetByIdAsync(sellerProfileId);
            if (seller == null)
                throw new ArgumentException("Invalid seller profile ID", nameof(sellerProfileId));

            // Generate a unique contract number
            var contractNumber = GenerateContractNumber();

            // Create a new contract instance from the template
            var contractInstance = new ContractInstance
            {
                ContractTemplateId = templateId,
                BuyerProfileId = buyerProfileId,
                SellerProfileId = sellerProfileId,
                RfqId = rfqId,
                QuoteId = quoteId,
                ContractNumber = contractNumber,
                Content = template.Content, // Use the template content as the base
                Status = "draft", // Default to draft status
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save the contract instance
            return await _contractInstanceRepository.CreateAsync(contractInstance);
        }

        /// <summary>
        /// Generates a unique contract number
        /// </summary>
        /// <returns>Unique contract number</returns>
        private string GenerateContractNumber()
        {
            // Generate a contract number with date and random component
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = new Random().Next(1000, 9999);
            return $"CT-{datePart}-{randomPart}";
        }
    }
}
