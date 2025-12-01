using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for certification data access operations
    /// </summary>
    public class CertificationRepository : ICertificationRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for CertificationRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public CertificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a certification by ID
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <returns>Certification entity if found, null otherwise</returns>
        public async Task<Certification?> GetByIdAsync(Guid id)
        {
            return await _context.Certifications
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Gets a certification by ID with seller profile included
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <returns>Certification entity if found, null otherwise</returns>
        public async Task<Certification?> GetByIdWithSellerProfileAsync(Guid id)
        {
            return await _context.Certifications
                .Include(c => c.SellerProfile)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Gets all certifications for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>List of certifications</returns>
        public async Task<List<Certification>> GetBySellerProfileIdAsync(Guid sellerProfileId)
        {
            return await _context.Certifications
                .Where(c => c.SellerProfileId == sellerProfileId)
                .OrderByDescending(c => c.SubmittedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets certifications by status
        /// </summary>
        /// <param name="status">Certification status to filter by</param>
        /// <returns>List of certifications with the specified status</returns>
        public async Task<List<Certification>> GetByStatusAsync(CertificationStatus status)
        {
            return await _context.Certifications
                .Include(c => c.SellerProfile)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.SubmittedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new certification
        /// </summary>
        /// <param name="certification">Certification entity to create</param>
        /// <returns>Created certification entity</returns>
        public async Task<Certification> CreateAsync(Certification certification)
        {
            var entry = await _context.Certifications.AddAsync(certification);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing certification
        /// </summary>
        /// <param name="certification">Certification entity to update</param>
        /// <returns>Updated certification entity</returns>
        public async Task<Certification> UpdateAsync(Certification certification)
        {
            var entry = _context.Certifications.Update(certification);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }
    }
}