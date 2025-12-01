using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for ContentCategory entities
    /// </summary>
    public interface IContentCategoryRepository
    {
        Task<ContentCategory> CreateAsync(ContentCategory category);
        Task<bool> UpdateAsync(ContentCategory category);
        Task<bool> DeleteAsync(Guid id);
        Task<ContentCategory?> GetByIdAsync(Guid id);
        Task<PagedResult<ContentCategory>> GetAllAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
        Task<ContentCategory?> GetBySlugAsync(string slug);
        Task<IEnumerable<ContentCategory>> GetAllActiveAsync();
    }

    /// <summary>
    /// Repository interface for ContentItem entities
    /// </summary>
    public interface IContentItemRepository
    {
        Task<ContentItem> CreateAsync(ContentItem contentItem);
        Task<bool> UpdateAsync(ContentItem contentItem);
        Task<bool> DeleteAsync(Guid id);
        Task<ContentItem?> GetByIdAsync(Guid id);
        Task<PagedResult<ContentItem>> GetAllAsync(int page, int pageSize, string? searchTerm = null, Guid? categoryId = null, string? contentType = null, bool? isActive = null, bool? isPublished = null, Guid? tagId = null);
        Task<ContentItem?> GetBySlugAsync(string slug);
        Task<IEnumerable<ContentItem>> GetPublishedByCategoryAsync(Guid categoryId);
        Task<IEnumerable<ContentItem>> GetScheduledForPublicationAsync();
        Task<IEnumerable<ContentItem>> GetScheduledForUnpublishingAsync();
        Task<IEnumerable<ContentItem>> GetByTagAsync(Guid tagId);
        Task<IEnumerable<ContentItem>> GetByDateRangeAsync(DateTime start, DateTime end);
    }

    /// <summary>
    /// Repository interface for ContentTag entities
    /// </summary>
    public interface IContentTagRepository
    {
        Task<ContentTag> CreateAsync(ContentTag tag);
        Task<bool> UpdateAsync(ContentTag tag);
        Task<bool> DeleteAsync(Guid id);
        Task<ContentTag?> GetByIdAsync(Guid id);
        Task<PagedResult<ContentTag>> GetAllAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
        Task<ContentTag?> GetBySlugAsync(string slug);
        Task<IEnumerable<ContentTag>> GetAllActiveAsync();
        Task<IEnumerable<ContentTag>> GetByContentItemIdAsync(Guid contentItemId);
        Task<bool> AddTagToContentItemAsync(Guid contentItemId, Guid tagId);
        Task<bool> RemoveTagFromContentItemAsync(Guid contentItemId, Guid tagId);
    }
}