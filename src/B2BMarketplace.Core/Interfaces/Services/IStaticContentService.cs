using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for static content operations
    /// </summary>
    public interface IStaticContentService
    {
        /// <summary>
        /// Gets a published static content page by its slug
        /// </summary>
        /// <param name="pageSlug">Page slug (e.g. 'terms', 'privacy', 'faq')</param>
        /// <returns>Static content page if found and published, null otherwise</returns>
        Task<StaticContent?> GetPublishedBySlugAsync(string pageSlug);

        /// <summary>
        /// Gets all published static content pages
        /// </summary>
        /// <returns>Collection of published static content pages</returns>
        Task<IEnumerable<StaticContent>> GetPublishedAsync();

        /// <summary>
        /// Gets all static content pages with pagination (typically for admin use)
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paginated result of static content pages</returns>
        Task<PagedResult<StaticContent>> GetAllAsync(int page, int pageSize);

        /// <summary>
        /// Gets static content by content type
        /// </summary>
        /// <param name="contentType">Type of content to retrieve</param>
        /// <returns>Collection of static content pages of specified type</returns>
        Task<IEnumerable<StaticContent>> GetByContentTypeAsync(string contentType);

        /// <summary>
        /// Creates a new static content page
        /// </summary>
        /// <param name="staticContent">Static content entity to create</param>
        /// <returns>Created static content entity</returns>
        Task<StaticContent> CreateAsync(StaticContent staticContent);

        /// <summary>
        /// Updates an existing static content page
        /// </summary>
        /// <param name="staticContent">Static content entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateAsync(StaticContent staticContent);
    }
}