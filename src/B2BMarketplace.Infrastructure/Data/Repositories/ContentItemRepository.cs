using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for ContentItem data access operations
    /// </summary>
    public class ContentItemRepository : IContentItemRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for ContentItemRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public ContentItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new content item in the database
        /// </summary>
        /// <param name="contentItem">Content item entity to create</param>
        /// <returns>Created content item entity</returns>
        public async Task<ContentItem> CreateAsync(ContentItem contentItem)
        {
            var entry = await _context.ContentItems.AddAsync(contentItem);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing content item in the database
        /// </summary>
        /// <param name="contentItem">Content item entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(ContentItem contentItem)
        {
            try
            {
                _context.ContentItems.Update(contentItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a content item by ID
        /// </summary>
        /// <param name="id">Content item ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var contentItem = await _context.ContentItems.FindAsync(id);
            if (contentItem == null)
                return false;

            _context.ContentItems.Remove(contentItem);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a content item by its ID
        /// </summary>
        /// <param name="id">Content item ID to search for</param>
        /// <returns>Content item entity if found, null otherwise</returns>
        public async Task<ContentItem?> GetByIdAsync(Guid id)
        {
            return await _context.ContentItems
                .Include(ci => ci.Category)
                .Include(ci => ci.Tags)
                .ThenInclude(t => t.ContentTag)
                .FirstOrDefaultAsync(ci => ci.Id == id);
        }

        /// <summary>
        /// Gets all content items with pagination and filtering
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="searchTerm">Search term to filter content items</param>
        /// <param name="categoryId">Filter by category ID</param>
        /// <param name="contentType">Filter by content type</param>
        /// <param name="isActive">Filter by active status</param>
        /// <param name="isPublished">Filter by published status</param>
        /// <param name="tagId">Filter by tag ID</param>
        /// <returns>Paged result containing content items</returns>
        public async Task<PagedResult<ContentItem>> GetAllAsync(int page, int pageSize, string? searchTerm = null, Guid? categoryId = null, string? contentType = null, bool? isActive = null, bool? isPublished = null, Guid? tagId = null)
        {
            // Ensure page and pageSize are valid
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit max page size

            // Build query
            var query = _context.ContentItems
                .Include(ci => ci.Category)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(ci => ci.Title.Contains(searchTerm) || ci.Content.Contains(searchTerm) || ci.Excerpt!.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(ci => ci.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(contentType))
            {
                query = query.Where(ci => ci.ContentType == contentType);
            }

            if (isActive.HasValue)
            {
                query = query.Where(ci => ci.IsActive == isActive.Value);
            }

            if (isPublished.HasValue)
            {
                query = query.Where(ci => ci.IsPublished == isPublished.Value);
            }

            if (tagId.HasValue)
            {
                query = query.Where(ci => ci.Tags.Any(t => t.ContentTagId == tagId.Value));
            }

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderByDescending(ci => ci.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ContentItem>(items, totalItems, page, pageSize);
        }

        /// <summary>
        /// Gets content item by slug
        /// </summary>
        /// <param name="slug">The content item slug</param>
        /// <returns>The content item or null</returns>
        public async Task<ContentItem?> GetBySlugAsync(string slug)
        {
            return await _context.ContentItems
                .Include(ci => ci.Category)
                .Include(ci => ci.Tags)
                .ThenInclude(t => t.ContentTag)
                .FirstOrDefaultAsync(ci => ci.Slug == slug);
        }

        /// <summary>
        /// Gets published content items by category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>Collection of published content items</returns>
        public async Task<IEnumerable<ContentItem>> GetPublishedByCategoryAsync(Guid categoryId)
        {
            return await _context.ContentItems
                .Where(ci => ci.CategoryId == categoryId && ci.IsPublished && ci.IsActive)
                .OrderByDescending(ci => ci.PublishedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets content items scheduled for publication
        /// </summary>
        /// <returns>Collection of content items scheduled for publication</returns>
        public async Task<IEnumerable<ContentItem>> GetScheduledForPublicationAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.ContentItems
                .Where(ci => ci.ScheduledPublishAt.HasValue &&
                            ci.ScheduledPublishAt <= now &&
                            !ci.IsPublished &&
                            ci.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Gets content items scheduled for unpublishing
        /// </summary>
        /// <returns>Collection of content items scheduled for unpublishing</returns>
        public async Task<IEnumerable<ContentItem>> GetScheduledForUnpublishingAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.ContentItems
                .Where(ci => ci.ScheduledUnpublishAt.HasValue &&
                            ci.ScheduledUnpublishAt <= now &&
                            ci.IsPublished)
                .ToListAsync();
        }

        /// <summary>
        /// Gets content items by tag
        /// </summary>
        /// <param name="tagId">Tag ID</param>
        /// <returns>Collection of content items with the specified tag</returns>
        public async Task<IEnumerable<ContentItem>> GetByTagAsync(Guid tagId)
        {
            return await _context.ContentItems
                .Where(ci => ci.Tags.Any(t => t.ContentTagId == tagId))
                .ToListAsync();
        }

        /// <summary>
        /// Gets content items by date range
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <returns>Collection of content items within the date range</returns>
        public async Task<IEnumerable<ContentItem>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.ContentItems
                .Where(ci => ci.CreatedAt >= start && ci.CreatedAt <= end)
                .ToListAsync();
        }
    }
}