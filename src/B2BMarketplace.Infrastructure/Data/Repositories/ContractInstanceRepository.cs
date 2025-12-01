using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for contract instance data access operations
    /// </summary>
    public class ContractInstanceRepository : IContractInstanceRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for ContractInstanceRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public ContractInstanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new contract instance in the database
        /// </summary>
        /// <param name="contractInstance">Contract instance entity to create</param>
        /// <returns>Created contract instance entity</returns>
        public async Task<ContractInstance> CreateAsync(ContractInstance contractInstance)
        {
            var entry = await _context.ContractInstances.AddAsync(contractInstance);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing contract instance in the database
        /// </summary>
        /// <param name="contractInstance">Contract instance entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(ContractInstance contractInstance)
        {
            try
            {
                _context.ContractInstances.Update(contractInstance);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a contract instance by ID
        /// </summary>
        /// <param name="id">Contract instance ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var contractInstance = await _context.ContractInstances.FindAsync(id);
            if (contractInstance == null)
                return false;

            _context.ContractInstances.Remove(contractInstance);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a contract instance by its ID
        /// </summary>
        /// <param name="id">Contract instance ID to search for</param>
        /// <returns>Contract instance entity if found, null otherwise</returns>
        public async Task<ContractInstance?> GetByIdAsync(Guid id)
        {
            return await _context.ContractInstances.FindAsync(id);
        }

        /// <summary>
        /// Gets all contract instances with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result containing contract instances</returns>
        public async Task<PagedResult<ContractInstance>> GetAllAsync(int page, int pageSize)
        {
            // Ensure page and pageSize are valid
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit max page size

            // Build query
            var query = _context.ContractInstances.AsQueryable();

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(ci => ci.ContractNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ContractInstance>(items, totalItems, page, pageSize);
        }

        /// <summary>
        /// Gets contract instances by buyer profile ID
        /// </summary>
        /// <param name="buyerProfileId">The buyer profile ID</param>
        /// <returns>Collection of contract instances</returns>
        public async Task<IEnumerable<ContractInstance>> GetByBuyerProfileIdAsync(Guid buyerProfileId)
        {
            return await _context.ContractInstances
                .Where(ci => ci.BuyerProfileId == buyerProfileId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets contract instances by seller profile ID
        /// </summary>
        /// <param name="sellerProfileId">The seller profile ID</param>
        /// <returns>Collection of contract instances</returns>
        public async Task<IEnumerable<ContractInstance>> GetBySellerProfileIdAsync(Guid sellerProfileId)
        {
            return await _context.ContractInstances
                .Where(ci => ci.SellerProfileId == sellerProfileId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a contract instance by its number
        /// </summary>
        /// <param name="contractNumber">The contract number</param>
        /// <returns>The contract instance or null</returns>
        public async Task<ContractInstance?> GetByNumberAsync(string contractNumber)
        {
            return await _context.ContractInstances
                .FirstOrDefaultAsync(ci => ci.ContractNumber == contractNumber);
        }
    }
}