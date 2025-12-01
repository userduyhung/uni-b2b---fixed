using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for static content operations
    /// </summary>
    public class StaticContentService : IStaticContentService
    {
        private readonly IStaticContentRepository _staticContentRepository;

        /// <summary>
        /// Constructor for StaticContentService
        /// </summary>
        /// <param name="staticContentRepository">Static content repository</param>
        public StaticContentService(IStaticContentRepository staticContentRepository)
        {
            _staticContentRepository = staticContentRepository;
        }

        /// <summary>
        /// Gets a published static content page by its slug
        /// </summary>
        /// <param name="pageSlug">Page slug (e.g. 'terms', 'privacy', 'faq')</param>
        /// <returns>Static content page if found and published, null otherwise</returns>
        public async Task<StaticContent?> GetPublishedBySlugAsync(string pageSlug)
        {
            return await _staticContentRepository.GetPublishedBySlugAsync(pageSlug);
        }

        /// <summary>
        /// Gets all published static content pages
        /// </summary>
        /// <returns>Collection of published static content pages</returns>
        public async Task<IEnumerable<StaticContent>> GetPublishedAsync()
        {
            return await _staticContentRepository.GetPublishedAsync();
        }

        /// <summary>
        /// Gets all static content pages with pagination (typically for admin use)
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paginated result of static content pages</returns>
        public async Task<PagedResult<StaticContent>> GetAllAsync(int page, int pageSize)
        {
            return await _staticContentRepository.GetAllAsync(page, pageSize);
        }

        /// <summary>
        /// Gets static content by content type
        /// </summary>
        /// <param name="contentType">Type of content to retrieve</param>
        /// <returns>Collection of static content pages of specified type</returns>
        public async Task<IEnumerable<StaticContent>> GetByContentTypeAsync(string contentType)
        {
            return await _staticContentRepository.GetByContentTypeAsync(contentType);
        }

        /// <summary>
        /// Creates a new static content page
        /// </summary>
        /// <param name="staticContent">Static content entity to create</param>
        /// <returns>Created static content entity</returns>
        public async Task<StaticContent> CreateAsync(StaticContent staticContent)
        {
            return await _staticContentRepository.CreateAsync(staticContent);
        }

        /// <summary>
        /// Updates an existing static content page
        /// </summary>
        /// <param name="staticContent">Static content entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(StaticContent staticContent)
        {
            return await _staticContentRepository.UpdateAsync(staticContent);
        }
    }
}
