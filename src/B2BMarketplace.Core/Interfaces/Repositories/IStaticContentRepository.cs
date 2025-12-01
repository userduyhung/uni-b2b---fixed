using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for StaticContent entities
    /// </summary>
    public interface IStaticContentRepository
    {
        /// <summary>
        /// Creates a new static content in the database
        /// </summary>
        /// <param name="staticContent">Static content entity to create</param>
        /// <returns>Created static content entity</returns>
        Task<StaticContent> CreateAsync(StaticContent staticContent);

        /// <summary>
        /// Updates an existing static content in the database
        /// </summary>
        /// <param name="staticContent">Static content entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateAsync(StaticContent staticContent);

        /// <summary>
        /// Deletes a static content by ID
        /// </summary>
        /// <param name="id">Static content ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Gets a static content by its ID
        /// </summary>
        /// <param name="id">Static content ID to search for</param>
        /// <returns>Static content entity if found, null otherwise</returns>
        Task<StaticContent?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all static content with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result containing static content</returns>
        Task<PagedResult<StaticContent>> GetAllAsync(int page, int pageSize);

        /// <summary>
        /// Gets static content by page slug
        /// </summary>
        /// <param name="pageSlug">The page slug</param>
        /// <returns>The static content or null</returns>
        Task<StaticContent?> GetBySlugAsync(string pageSlug);

        /// <summary>
        /// Gets published static content by page slug
        /// </summary>
        /// <param name="pageSlug">The page slug</param>
        /// <returns>The published static content or null</returns>
        Task<StaticContent?> GetPublishedBySlugAsync(string pageSlug);

        /// <summary>
        /// Gets static content by content type
        /// </summary>
        /// <param name="contentType">The content type</param>
        /// <returns>Collection of static content</returns>
        Task<IEnumerable<StaticContent>> GetByContentTypeAsync(string contentType);

        /// <summary>
        /// Gets all published static content
        /// </summary>
        /// <returns>Collection of published static content</returns>
        Task<IEnumerable<StaticContent>> GetPublishedAsync();
    }
}