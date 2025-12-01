using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for contract template data access operations
    /// </summary>
    public class ContractTemplateRepository : IContractTemplateRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for ContractTemplateRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public ContractTemplateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new contract template in the database
        /// </summary>
        /// <param name="contractTemplate">Contract template entity to create</param>
        /// <returns>Created contract template entity</returns>
        public async Task<ContractTemplate> CreateAsync(ContractTemplate contractTemplate)
        {
            var entry = await _context.ContractTemplates.AddAsync(contractTemplate);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing contract template in the database
        /// </summary>
        /// <param name="contractTemplate">Contract template entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(ContractTemplate contractTemplate)
        {
            try
            {
                _context.ContractTemplates.Update(contractTemplate);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a contract template by ID
        /// </summary>
        /// <param name="id">Contract template ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var contractTemplate = await _context.ContractTemplates.FindAsync(id);
            if (contractTemplate == null)
                return false;

            _context.ContractTemplates.Remove(contractTemplate);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a contract template by its ID
        /// </summary>
        /// <param name="id">Contract template ID to search for</param>
        /// <returns>Contract template entity if found, null otherwise</returns>
        public async Task<ContractTemplate?> GetByIdAsync(Guid id)
        {
            return await _context.ContractTemplates.FindAsync(id);
        }

        /// <summary>
        /// Gets all contract templates with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result containing contract templates</returns>
        public async Task<PagedResult<ContractTemplate>> GetAllAsync(int page, int pageSize)
        {
            // Ensure page and pageSize are valid
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit max page size

            // Build query
            var query = _context.ContractTemplates.AsQueryable();

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(ct => ct.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ContractTemplate>(items, totalItems, page, pageSize);
        }

        /// <summary>
        /// Gets active contract templates by seller profile ID
        /// </summary>
        /// <param name="sellerProfileId">The seller profile ID</param>
        /// <returns>Collection of active contract templates</returns>
        public async Task<IEnumerable<ContractTemplate>> GetActiveBySellerProfileIdAsync(Guid sellerProfileId)
        {
            return await _context.ContractTemplates
                .Where(ct => ct.CreatedBySellerProfileId == sellerProfileId && ct.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a contract template by its ID with navigation properties
        /// </summary>
        /// <param name="id">The contract template ID</param>
        /// <returns>The contract template with navigation properties</returns>
        public async Task<ContractTemplate?> GetByIdWithNavigationAsync(Guid id)
        {
            return await _context.ContractTemplates
                .Include(ct => ct.CreatedBySellerProfile)
                .Include(ct => ct.ContractInstances)
                .FirstOrDefaultAsync(ct => ct.Id == id);
        }
    }
}