using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for ContentCategory operations
    /// </summary>
    public interface IContentCategoryService
    {
        Task<ContentCategoryDto> CreateCategoryAsync(CreateContentCategoryDto createCategoryDto, string userId);
        Task<ContentCategoryDto> UpdateCategoryAsync(Guid id, UpdateContentCategoryDto updateCategoryDto, string userId);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<ContentCategoryDto?> GetCategoryByIdAsync(Guid id);
        Task<PagedResult<ContentCategoryDto>> GetAllCategoriesAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
        Task<ContentCategoryDto?> GetCategoryBySlugAsync(string slug);
        Task<IEnumerable<ContentCategoryDto>> GetAllActiveCategoriesAsync();
    }

    /// <summary>
    /// Service interface for ContentItem operations
    /// </summary>
    public interface IContentItemService
    {
        Task<ContentItemDto> CreateContentItemAsync(CreateContentItemDto createContentItemDto, string userId);
        Task<ContentItemDto> UpdateContentItemAsync(Guid id, UpdateContentItemDto updateContentItemDto, string userId);
        Task<bool> DeleteContentItemAsync(Guid id);
        Task<ContentItemDto?> GetContentItemByIdAsync(Guid id);
        Task<ContentItemDto?> GetContentItemBySlugAsync(string slug);
        Task<PagedResult<ContentItemDto>> GetAllContentItemsAsync(ContentItemFilterDto filter);
        Task<IEnumerable<ContentItemDto>> GetPublishedContentItemsByCategoryAsync(Guid categoryId);
        Task<bool> PublishContentItemAsync(Guid id, string userId);
        Task<bool> UnpublishContentItemAsync(Guid id, string userId);
        Task ProcessScheduledContentAsync();
    }

    /// <summary>
    /// Service interface for ContentTag operations
    /// </summary>
    public interface IContentTagService
    {
        Task<ContentTagDto> CreateTagAsync(CreateContentTagDto createTagDto, string userId);
        Task<ContentTagDto> UpdateTagAsync(Guid id, UpdateContentTagDto updateTagDto, string userId);
        Task<bool> DeleteTagAsync(Guid id);
        Task<ContentTagDto?> GetTagByIdAsync(Guid id);
        Task<PagedResult<ContentTagDto>> GetAllTagsAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
        Task<ContentTagDto?> GetTagBySlugAsync(string slug);
        Task<IEnumerable<ContentTagDto>> GetAllActiveTagsAsync();
        Task<IEnumerable<ContentTagDto>> GetTagsByContentItemIdAsync(Guid contentItemId);
        Task<bool> AddTagToContentItemAsync(Guid contentItemId, Guid tagId);
        Task<bool> RemoveTagFromContentItemAsync(Guid contentItemId, Guid tagId);
    }
}