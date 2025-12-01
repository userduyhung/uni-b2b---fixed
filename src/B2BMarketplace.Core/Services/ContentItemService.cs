using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Models;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for ContentItem operations
    /// </summary>
    public class ContentItemService : IContentItemService
    {
        private readonly IContentItemRepository _contentItemRepository;
        private readonly IContentCategoryRepository _contentCategoryRepository;
        private readonly IContentTagRepository _contentTagRepository;

        /// <summary>
        /// Constructor for ContentItemService
        /// </summary>
        /// <param name="contentItemRepository">Content item repository</param>
        /// <param name="contentCategoryRepository">Content category repository</param>
        /// <param name="contentTagRepository">Content tag repository</param>
        public ContentItemService(
            IContentItemRepository contentItemRepository,
            IContentCategoryRepository contentCategoryRepository,
            IContentTagRepository contentTagRepository)
        {
            _contentItemRepository = contentItemRepository;
            _contentCategoryRepository = contentCategoryRepository;
            _contentTagRepository = contentTagRepository;
        }

        /// <summary>
        /// Creates a new content item
        /// </summary>
        /// <param name="createContentItemDto">Creation DTO</param>
        /// <param name="userId">ID of the user creating the content item</param>
        /// <returns>Created content item DTO</returns>
        public async Task<ContentItemDto> CreateContentItemAsync(CreateContentItemDto createContentItemDto, string userId)
        {
            var contentItem = new ContentItem
            {
                Title = createContentItemDto.Title,
                Content = createContentItemDto.Content,
                Slug = createContentItemDto.Slug,
                Excerpt = createContentItemDto.Excerpt,
                MetaTitle = createContentItemDto.MetaTitle,
                MetaDescription = createContentItemDto.MetaDescription,
                ContentType = createContentItemDto.ContentType,
                CategoryId = createContentItemDto.CategoryId,
                IsActive = createContentItemDto.IsActive,
                IsPublished = createContentItemDto.IsPublished,
                PublishedAt = createContentItemDto.PublishedAt,
                ScheduledPublishAt = createContentItemDto.ScheduledPublishAt,
                ScheduledUnpublishAt = createContentItemDto.ScheduledUnpublishAt,
                CreatedBy = userId,
                UpdatedBy = userId
            };

            // If published and no published date set, set it to now
            if (contentItem.IsPublished && !contentItem.PublishedAt.HasValue)
            {
                contentItem.PublishedAt = DateTime.UtcNow;
            }

            var createdContentItem = await _contentItemRepository.CreateAsync(contentItem);

            // Add tags
            foreach (var tagId in createContentItemDto.TagIds)
            {
                await _contentTagRepository.AddTagToContentItemAsync(createdContentItem.Id, tagId);
            }

            // Refresh the entity to get tags
            var refreshedContentItem = await _contentItemRepository.GetByIdAsync(createdContentItem.Id);

            return MapToDto(refreshedContentItem!);
        }

        /// <summary>
        /// Updates an existing content item
        /// </summary>
        /// <param name="id">Content item ID</param>
        /// <param name="updateContentItemDto">Update DTO</param>
        /// <param name="userId">ID of the user updating the content item</param>
        /// <returns>Updated content item DTO</returns>
        public async Task<ContentItemDto> UpdateContentItemAsync(Guid id, UpdateContentItemDto updateContentItemDto, string userId)
        {
            var contentItem = await _contentItemRepository.GetByIdAsync(id);
            if (contentItem == null)
            {
                throw new ArgumentException("Content item not found", nameof(id));
            }

            contentItem.Title = updateContentItemDto.Title;
            contentItem.Content = updateContentItemDto.Content;
            contentItem.Slug = updateContentItemDto.Slug;
            contentItem.Excerpt = updateContentItemDto.Excerpt;
            contentItem.MetaTitle = updateContentItemDto.MetaTitle;
            contentItem.MetaDescription = updateContentItemDto.MetaDescription;
            contentItem.ContentType = updateContentItemDto.ContentType;
            contentItem.CategoryId = updateContentItemDto.CategoryId;
            contentItem.IsActive = updateContentItemDto.IsActive;
            contentItem.IsPublished = updateContentItemDto.IsPublished;
            contentItem.PublishedAt = updateContentItemDto.PublishedAt;
            contentItem.ScheduledPublishAt = updateContentItemDto.ScheduledPublishAt;
            contentItem.ScheduledUnpublishAt = updateContentItemDto.ScheduledUnpublishAt;
            contentItem.UpdatedBy = userId;
            contentItem.UpdatedAt = DateTime.UtcNow;

            // If published and no published date set, set it to now
            if (contentItem.IsPublished && !contentItem.PublishedAt.HasValue)
            {
                contentItem.PublishedAt = DateTime.UtcNow;
            }

            var success = await _contentItemRepository.UpdateAsync(contentItem);
            if (!success)
            {
                throw new InvalidOperationException("Failed to update content item");
            }

            // Update tags
            // First remove all existing tags
            foreach (var tag in contentItem.Tags.ToList())
            {
                await _contentTagRepository.RemoveTagFromContentItemAsync(contentItem.Id, tag.ContentTagId);
            }

            // Then add new tags
            foreach (var tagId in updateContentItemDto.TagIds)
            {
                await _contentTagRepository.AddTagToContentItemAsync(contentItem.Id, tagId);
            }

            // Refresh the entity to get tags
            var refreshedContentItem = await _contentItemRepository.GetByIdAsync(contentItem.Id);

            return MapToDto(refreshedContentItem!);
        }

        /// <summary>
        /// Deletes a content item
        /// </summary>
        /// <param name="id">Content item ID</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> DeleteContentItemAsync(Guid id)
        {
            return await _contentItemRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Gets a content item by ID
        /// </summary>
        /// <param name="id">Content item ID</param>
        /// <returns>Content item DTO or null if not found</returns>
        public async Task<ContentItemDto?> GetContentItemByIdAsync(Guid id)
        {
            var contentItem = await _contentItemRepository.GetByIdAsync(id);
            if (contentItem == null)
                return null;

            return MapToDto(contentItem);
        }

        /// <summary>
        /// Gets a content item by slug
        /// </summary>
        /// <param name="slug">Content item slug</param>
        /// <returns>Content item DTO or null if not found</returns>
        public async Task<ContentItemDto?> GetContentItemBySlugAsync(string slug)
        {
            var contentItem = await _contentItemRepository.GetBySlugAsync(slug);
            if (contentItem == null)
                return null;

            return MapToDto(contentItem);
        }

        /// <summary>
        /// Gets all content items with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter DTO</param>
        /// <returns>Paged result of content item DTOs</returns>
        public async Task<PagedResult<ContentItemDto>> GetAllContentItemsAsync(ContentItemFilterDto filter)
        {
            var pagedResult = await _contentItemRepository.GetAllAsync(
                filter.Page,
                filter.PageSize,
                filter.SearchTerm,
                filter.CategoryId,
                filter.ContentType,
                filter.IsActive,
                filter.IsPublished,
                filter.TagId);

            var contentItemDtos = pagedResult.Items.Select(MapToDto).ToList();

            return new PagedResult<ContentItemDto>(contentItemDtos, pagedResult.TotalItems, pagedResult.CurrentPage, pagedResult.PageSize);
        }

        /// <summary>
        /// Gets published content items by category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>Collection of published content item DTOs</returns>
        public async Task<IEnumerable<ContentItemDto>> GetPublishedContentItemsByCategoryAsync(Guid categoryId)
        {
            var contentItems = await _contentItemRepository.GetPublishedByCategoryAsync(categoryId);

            return contentItems.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Publishes a content item
        /// </summary>
        /// <param name="id">Content item ID</param>
        /// <param name="userId">ID of the user publishing the content item</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> PublishContentItemAsync(Guid id, string userId)
        {
            var contentItem = await _contentItemRepository.GetByIdAsync(id);
            if (contentItem == null)
                return false;

            contentItem.IsPublished = true;
            contentItem.PublishedAt = contentItem.PublishedAt ?? DateTime.UtcNow;
            contentItem.UpdatedBy = userId;
            contentItem.UpdatedAt = DateTime.UtcNow;

            return await _contentItemRepository.UpdateAsync(contentItem);
        }

        /// <summary>
        /// Unpublishes a content item
        /// </summary>
        /// <param name="id">Content item ID</param>
        /// <param name="userId">ID of the user unpublishing the content item</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> UnpublishContentItemAsync(Guid id, string userId)
        {
            var contentItem = await _contentItemRepository.GetByIdAsync(id);
            if (contentItem == null)
                return false;

            contentItem.IsPublished = false;
            contentItem.UpdatedBy = userId;
            contentItem.UpdatedAt = DateTime.UtcNow;

            return await _contentItemRepository.UpdateAsync(contentItem);
        }

        /// <summary>
        /// Processes scheduled content for publication/unpublication
        /// </summary>
        /// <returns>Task</returns>
        public async Task ProcessScheduledContentAsync()
        {
            // Process scheduled publications
            var itemsToPublish = await _contentItemRepository.GetScheduledForPublicationAsync();
            foreach (var item in itemsToPublish)
            {
                item.IsPublished = true;
                item.PublishedAt = item.PublishedAt ?? DateTime.UtcNow;
                item.UpdatedAt = DateTime.UtcNow;
                await _contentItemRepository.UpdateAsync(item);
            }

            // Process scheduled unpublications
            var itemsToUnpublish = await _contentItemRepository.GetScheduledForUnpublishingAsync();
            foreach (var item in itemsToUnpublish)
            {
                item.IsPublished = false;
                item.UpdatedAt = DateTime.UtcNow;
                await _contentItemRepository.UpdateAsync(item);
            }
        }

        /// <summary>
        /// Maps a ContentItem entity to a ContentItemDto
        /// </summary>
        /// <param name="contentItem">ContentItem entity</param>
        /// <returns>ContentItemDto</returns>
        private ContentItemDto MapToDto(ContentItem contentItem)
        {
            var categoryDto = contentItem.Category != null ? new ContentCategoryDto
            {
                Id = contentItem.Category.Id,
                Name = contentItem.Category.Name,
                Description = contentItem.Category.Description,
                Slug = contentItem.Category.Slug,
                DisplayOrder = contentItem.Category.DisplayOrder,
                IsActive = contentItem.Category.IsActive,
                CreatedAt = contentItem.Category.CreatedAt,
                UpdatedAt = contentItem.Category.UpdatedAt
            } : null;

            var tagDtos = contentItem.Tags?.Where(t => t.ContentTag != null).Select(t => 
            {
                var tag = t.ContentTag!;
                return new ContentTagDto
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Slug = tag.Slug,
                    Description = tag.Description,
                    IsActive = tag.IsActive,
                    CreatedAt = tag.CreatedAt,
                    UpdatedAt = tag.UpdatedAt
                };
            }).ToList() ?? new List<ContentTagDto>();

            return new ContentItemDto
            {
                Id = contentItem.Id,
                Title = contentItem.Title,
                Content = contentItem.Content,
                Slug = contentItem.Slug,
                Excerpt = contentItem.Excerpt,
                MetaTitle = contentItem.MetaTitle,
                MetaDescription = contentItem.MetaDescription,
                ContentType = contentItem.ContentType,
                CategoryId = contentItem.CategoryId,
                Category = categoryDto,
                IsActive = contentItem.IsActive,
                IsPublished = contentItem.IsPublished,
                PublishedAt = contentItem.PublishedAt,
                ScheduledPublishAt = contentItem.ScheduledPublishAt,
                ScheduledUnpublishAt = contentItem.ScheduledUnpublishAt,
                CreatedAt = contentItem.CreatedAt,
                UpdatedAt = contentItem.UpdatedAt,
                CreatedBy = contentItem.CreatedBy,
                UpdatedBy = contentItem.UpdatedBy,
                Tags = tagDtos
            };
        }
    }
}
