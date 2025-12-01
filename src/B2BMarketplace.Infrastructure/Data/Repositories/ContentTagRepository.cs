using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for ContentTag data access operations
    /// </summary>
    public class ContentTagRepository : IContentTagRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for ContentTagRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public ContentTagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new content tag in the database
        /// </summary>
        /// <param name="tag">Content tag entity to create</param>
        /// <returns>Created content tag entity</returns>
        public async Task<ContentTag> CreateAsync(ContentTag tag)
        {
            var entry = await _context.ContentTags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing content tag in the database
        /// </summary>
        /// <param name="tag">Content tag entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(ContentTag tag)
        {
            try
            {
                _context.ContentTags.Update(tag);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a content tag by ID
        /// </summary>
        /// <param name="id">Content tag ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var tag = await _context.ContentTags.FindAsync(id);
            if (tag == null)
                return false;

            _context.ContentTags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a content tag by its ID
        /// </summary>
        /// <param name="id">Content tag ID to search for</param>
        /// <returns>Content tag entity if found, null otherwise</returns>
        public async Task<ContentTag?> GetByIdAsync(Guid id)
        {
            return await _context.ContentTags.FindAsync(id);
        }

        /// <summary>
        /// Gets all content tags with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="searchTerm">Search term to filter tags</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paged result containing content tags</returns>
        public async Task<PagedResult<ContentTag>> GetAllAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
        {
            // Ensure page and pageSize are valid
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit max page size

            // Build query
            var query = _context.ContentTags.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t => t.Name.Contains(searchTerm) || t.Description!.Contains(searchTerm));
            }

            if (isActive.HasValue)
            {
                query = query.Where(t => t.IsActive == isActive.Value);
            }

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(t => t.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ContentTag>(items, totalItems, page, pageSize);
        }

        /// <summary>
        /// Gets content tag by slug
        /// </summary>
        /// <param name="slug">The tag slug</param>
        /// <returns>The content tag or null</returns>
        public async Task<ContentTag?> GetBySlugAsync(string slug)
        {
            return await _context.ContentTags
                .FirstOrDefaultAsync(t => t.Slug == slug);
        }

        /// <summary>
        /// Gets all active content tags
        /// </summary>
        /// <returns>Collection of active content tags</returns>
        public async Task<IEnumerable<ContentTag>> GetAllActiveAsync()
        {
            return await _context.ContentTags
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Gets tags by content item ID
        /// </summary>
        /// <param name="contentItemId">Content item ID</param>
        /// <returns>Collection of tags associated with the content item</returns>
        public async Task<IEnumerable<ContentTag>> GetByContentItemIdAsync(Guid contentItemId)
        {
            return await _context.ContentTags
                .Where(t => t.ContentItems.Any(cit => cit.ContentItemId == contentItemId))
                .ToListAsync();
        }

        /// <summary>
        /// Adds a tag to a content item
        /// </summary>
        /// <param name="contentItemId">Content item ID</param>
        /// <param name="tagId">Tag ID</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> AddTagToContentItemAsync(Guid contentItemId, Guid tagId)
        {
            try
            {
                // Check if the relationship already exists
                var exists = await _context.ContentItemTags
                    .AnyAsync(cit => cit.ContentItemId == contentItemId && cit.ContentTagId == tagId);

                if (!exists)
                {
                    var contentItemTag = new ContentItemTag
                    {
                        ContentItemId = contentItemId,
                        ContentTagId = tagId
                    };

                    await _context.ContentItemTags.AddAsync(contentItemTag);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes a tag from a content item
        /// </summary>
        /// <param name="contentItemId">Content item ID</param>
        /// <param name="tagId">Tag ID</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> RemoveTagFromContentItemAsync(Guid contentItemId, Guid tagId)
        {
            try
            {
                var contentItemTag = await _context.ContentItemTags
                    .FirstOrDefaultAsync(cit => cit.ContentItemId == contentItemId && cit.ContentTagId == tagId);

                if (contentItemTag != null)
                {
                    _context.ContentItemTags.Remove(contentItemTag);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}