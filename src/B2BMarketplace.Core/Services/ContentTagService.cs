using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Models;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for ContentTag operations
    /// </summary>
    public class ContentTagService : IContentTagService
    {
        private readonly IContentTagRepository _contentTagRepository;

        /// <summary>
        /// Constructor for ContentTagService
        /// </summary>
        /// <param name="contentTagRepository">Content tag repository</param>
        public ContentTagService(IContentTagRepository contentTagRepository)
        {
            _contentTagRepository = contentTagRepository;
        }

        /// <summary>
        /// Creates a new content tag
        /// </summary>
        /// <param name="createTagDto">Creation DTO</param>
        /// <param name="userId">ID of the user creating the tag</param>
        /// <returns>Created tag DTO</returns>
        public async Task<ContentTagDto> CreateTagAsync(CreateContentTagDto createTagDto, string userId)
        {
            var tag = new ContentTag
            {
                Name = createTagDto.Name,
                Slug = createTagDto.Slug,
                Description = createTagDto.Description,
                IsActive = createTagDto.IsActive
            };

            var createdTag = await _contentTagRepository.CreateAsync(tag);

            return new ContentTagDto
            {
                Id = createdTag.Id,
                Name = createdTag.Name,
                Slug = createdTag.Slug,
                Description = createdTag.Description,
                IsActive = createdTag.IsActive,
                CreatedAt = createdTag.CreatedAt,
                UpdatedAt = createdTag.UpdatedAt
            };
        }

        /// <summary>
        /// Updates an existing content tag
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <param name="updateTagDto">Update DTO</param>
        /// <param name="userId">ID of the user updating the tag</param>
        /// <returns>Updated tag DTO</returns>
        public async Task<ContentTagDto> UpdateTagAsync(Guid id, UpdateContentTagDto updateTagDto, string userId)
        {
            var tag = await _contentTagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                throw new ArgumentException("Tag not found", nameof(id));
            }

            tag.Name = updateTagDto.Name;
            tag.Slug = updateTagDto.Slug;
            tag.Description = updateTagDto.Description;
            tag.IsActive = updateTagDto.IsActive;
            tag.UpdatedAt = DateTime.UtcNow;

            var success = await _contentTagRepository.UpdateAsync(tag);
            if (!success)
            {
                throw new InvalidOperationException("Failed to update tag");
            }

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
        }

        /// <summary>
        /// Deletes a content tag
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> DeleteTagAsync(Guid id)
        {
            return await _contentTagRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Gets a content tag by ID
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <returns>Tag DTO or null if not found</returns>
        public async Task<ContentTagDto?> GetTagByIdAsync(Guid id)
        {
            var tag = await _contentTagRepository.GetByIdAsync(id);
            if (tag == null)
                return null;

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
        }

        /// <summary>
        /// Gets all content tags with pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="searchTerm">Search term</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paged result of tag DTOs</returns>
        public async Task<PagedResult<ContentTagDto>> GetAllTagsAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
        {
            var pagedResult = await _contentTagRepository.GetAllAsync(page, pageSize, searchTerm, isActive);

            var tagDtos = pagedResult.Items.Select(tag => new ContentTagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug,
                Description = tag.Description,
                IsActive = tag.IsActive,
                CreatedAt = tag.CreatedAt,
                UpdatedAt = tag.UpdatedAt
            }).ToList();

            return new PagedResult<ContentTagDto>(tagDtos, pagedResult.TotalItems, pagedResult.CurrentPage, pagedResult.PageSize);
        }

        /// <summary>
        /// Gets a content tag by slug
        /// </summary>
        /// <param name="slug">Tag slug</param>
        /// <returns>Tag DTO or null if not found</returns>
        public async Task<ContentTagDto?> GetTagBySlugAsync(string slug)
        {
            var tag = await _contentTagRepository.GetBySlugAsync(slug);
            if (tag == null)
                return null;

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
        }

        /// <summary>
        /// Gets all active content tags
        /// </summary>
        /// <returns>Collection of active tag DTOs</returns>
        public async Task<IEnumerable<ContentTagDto>> GetAllActiveTagsAsync()
        {
            var tags = await _contentTagRepository.GetAllActiveAsync();

            return tags.Select(tag => new ContentTagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug,
                Description = tag.Description,
                IsActive = tag.IsActive,
                CreatedAt = tag.CreatedAt,
                UpdatedAt = tag.UpdatedAt
            }).ToList();
        }

        /// <summary>
        /// Gets tags by content item ID
        /// </summary>
        /// <param name="contentItemId">Content item ID</param>
        /// <returns>Collection of tag DTOs</returns>
        public async Task<IEnumerable<ContentTagDto>> GetTagsByContentItemIdAsync(Guid contentItemId)
        {
            var tags = await _contentTagRepository.GetByContentItemIdAsync(contentItemId);

            return tags.Select(tag => new ContentTagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug,
                Description = tag.Description,
                IsActive = tag.IsActive,
                CreatedAt = tag.CreatedAt,
                UpdatedAt = tag.UpdatedAt
            }).ToList();
        }

        /// <summary>
        /// Adds a tag to a content item
        /// </summary>
        /// <param name="contentItemId">Content item ID</param>
        /// <param name="tagId">Tag ID</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> AddTagToContentItemAsync(Guid contentItemId, Guid tagId)
        {
            return await _contentTagRepository.AddTagToContentItemAsync(contentItemId, tagId);
        }

        /// <summary>
        /// Removes a tag from a content item
        /// </summary>
        /// <param name="contentItemId">Content item ID</param>
        /// <param name="tagId">Tag ID</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> RemoveTagFromContentItemAsync(Guid contentItemId, Guid tagId)
        {
            return await _contentTagRepository.RemoveTagFromContentItemAsync(contentItemId, tagId);
        }
    }
}