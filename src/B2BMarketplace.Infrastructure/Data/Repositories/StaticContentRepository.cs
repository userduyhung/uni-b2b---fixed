using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for static content data access operations
    /// </summary>
    public class StaticContentRepository : IStaticContentRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for StaticContentRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public StaticContentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new static content in the database
        /// </summary>
        /// <param name="staticContent">Static content entity to create</param>
        /// <returns>Created static content entity</returns>
        public async Task<StaticContent> CreateAsync(StaticContent staticContent)
        {
            var entry = await _context.StaticContents.AddAsync(staticContent);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing static content in the database
        /// </summary>
        /// <param name="staticContent">Static content entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(StaticContent staticContent)
        {
            try
            {
                _context.StaticContents.Update(staticContent);
                await _context.SaveChangesAsync();
                staticContent.UpdatedAt = DateTime.UtcNow; // Update timestamp
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a static content by ID
        /// </summary>
        /// <param name="id">Static content ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var staticContent = await _context.StaticContents.FindAsync(id);
            if (staticContent == null)
                return false;

            _context.StaticContents.Remove(staticContent);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a static content by its ID
        /// </summary>
        /// <param name="id">Static content ID to search for</param>
        /// <returns>Static content entity if found, null otherwise</returns>
        public async Task<StaticContent?> GetByIdAsync(Guid id)
        {
            return await _context.StaticContents.FindAsync(id);
        }

        /// <summary>
        /// Gets all static content with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result containing static content</returns>
        public async Task<PagedResult<StaticContent>> GetAllAsync(int page, int pageSize)
        {
            // Ensure page and pageSize are valid
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit max page size

            // Build query
            var query = _context.StaticContents.AsQueryable();

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(sc => sc.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<StaticContent>(items, totalItems, page, pageSize);
        }

        /// <summary>
        /// Gets static content by page slug
        /// </summary>
        /// <param name="pageSlug">The page slug</param>
        /// <returns>The static content or null</returns>
        public async Task<StaticContent?> GetBySlugAsync(string pageSlug)
        {
            return await _context.StaticContents
                .FirstOrDefaultAsync(sc => sc.PageSlug == pageSlug);
        }

        /// <summary>
        /// Gets published static content by page slug
        /// </summary>
        /// <param name="pageSlug">The page slug</param>
        /// <returns>The published static content or null</returns>
        public async Task<StaticContent?> GetPublishedBySlugAsync(string pageSlug)
        {
            return await _context.StaticContents
                .FirstOrDefaultAsync(sc => sc.PageSlug == pageSlug && sc.IsPublished && sc.IsActive);
        }

        /// <summary>
        /// Gets static content by content type
        /// </summary>
        /// <param name="contentType">The content type</param>
        /// <returns>Collection of static content</returns>
        public async Task<IEnumerable<StaticContent>> GetByContentTypeAsync(string contentType)
        {
            return await _context.StaticContents
                .Where(sc => sc.ContentType == contentType)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all published static content
        /// </summary>
        /// <returns>Collection of published static content</returns>
        public async Task<IEnumerable<StaticContent>> GetPublishedAsync()
        {
            return await _context.StaticContents
                .Where(sc => sc.IsPublished && sc.IsActive)
                .ToListAsync();
        }
    }
}