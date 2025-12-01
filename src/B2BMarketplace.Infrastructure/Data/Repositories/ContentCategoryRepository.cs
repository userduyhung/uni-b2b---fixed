using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for ContentCategory data access operations
    /// </summary>
    public class ContentCategoryRepository : IContentCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for ContentCategoryRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public ContentCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new content category in the database
        /// </summary>
        /// <param name="category">Content category entity to create</param>
        /// <returns>Created content category entity</returns>
        public async Task<ContentCategory> CreateAsync(ContentCategory category)
        {
            var entry = await _context.ContentCategories.AddAsync(category);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing content category in the database
        /// </summary>
        /// <param name="category">Content category entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(ContentCategory category)
        {
            try
            {
                _context.ContentCategories.Update(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a content category by ID
        /// </summary>
        /// <param name="id">Content category ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _context.ContentCategories.FindAsync(id);
            if (category == null)
                return false;

            _context.ContentCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a content category by its ID
        /// </summary>
        /// <param name="id">Content category ID to search for</param>
        /// <returns>Content category entity if found, null otherwise</returns>
        public async Task<ContentCategory?> GetByIdAsync(Guid id)
        {
            return await _context.ContentCategories.FindAsync(id);
        }

        /// <summary>
        /// Gets all content categories with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="searchTerm">Search term to filter categories</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paged result containing content categories</returns>
        public async Task<PagedResult<ContentCategory>> GetAllAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
        {
            // Ensure page and pageSize are valid
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit max page size

            // Build query
            var query = _context.ContentCategories.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm) || c.Description!.Contains(searchTerm));
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ContentCategory>(items, totalItems, page, pageSize);
        }

        /// <summary>
        /// Gets content category by slug
        /// </summary>
        /// <param name="slug">The category slug</param>
        /// <returns>The content category or null</returns>
        public async Task<ContentCategory?> GetBySlugAsync(string slug)
        {
            return await _context.ContentCategories
                .FirstOrDefaultAsync(c => c.Slug == slug);
        }

        /// <summary>
        /// Gets all active content categories
        /// </summary>
        /// <returns>Collection of active content categories</returns>
        public async Task<IEnumerable<ContentCategory>> GetAllActiveAsync()
        {
            return await _context.ContentCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }
    }
}