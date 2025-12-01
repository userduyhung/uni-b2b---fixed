using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Models;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for ContentCategory operations
    /// </summary>
    public class ContentCategoryService : IContentCategoryService
    {
        private readonly IContentCategoryRepository _contentCategoryRepository;

        /// <summary>
        /// Constructor for ContentCategoryService
        /// </summary>
        /// <param name="contentCategoryRepository">Content category repository</param>
        public ContentCategoryService(IContentCategoryRepository contentCategoryRepository)
        {
            _contentCategoryRepository = contentCategoryRepository;
        }

        /// <summary>
        /// Creates a new content category
        /// </summary>
        /// <param name="createCategoryDto">Creation DTO</param>
        /// <param name="userId">ID of the user creating the category</param>
        /// <returns>Created category DTO</returns>
        public async Task<ContentCategoryDto> CreateCategoryAsync(CreateContentCategoryDto createCategoryDto, string userId)
        {
            var category = new ContentCategory
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                Slug = createCategoryDto.Slug,
                DisplayOrder = createCategoryDto.DisplayOrder,
                IsActive = createCategoryDto.IsActive,
                CreatedById = Guid.Parse(userId),
                UpdatedById = Guid.Parse(userId)
            };

            var createdCategory = await _contentCategoryRepository.CreateAsync(category);

            return new ContentCategoryDto
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name,
                Description = createdCategory.Description,
                Slug = createdCategory.Slug,
                DisplayOrder = createdCategory.DisplayOrder,
                IsActive = createdCategory.IsActive,
                CreatedAt = createdCategory.CreatedAt,
                UpdatedAt = createdCategory.UpdatedAt
            };
        }

        /// <summary>
        /// Updates an existing content category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="updateCategoryDto">Update DTO</param>
        /// <param name="userId">ID of the user updating the category</param>
        /// <returns>Updated category DTO</returns>
        public async Task<ContentCategoryDto> UpdateCategoryAsync(Guid id, UpdateContentCategoryDto updateCategoryDto, string userId)
        {
            var category = await _contentCategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new ArgumentException("Category not found", nameof(id));
            }

            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;
            category.Slug = updateCategoryDto.Slug;
            category.DisplayOrder = updateCategoryDto.DisplayOrder;
            category.IsActive = updateCategoryDto.IsActive;
            category.UpdatedById = Guid.Parse(userId);
            category.UpdatedAt = DateTime.UtcNow;

            var success = await _contentCategoryRepository.UpdateAsync(category);
            if (!success)
            {
                throw new InvalidOperationException("Failed to update category");
            }

            return new ContentCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        /// <summary>
        /// Deletes a content category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            return await _contentCategoryRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Gets a content category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category DTO or null if not found</returns>
        public async Task<ContentCategoryDto?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _contentCategoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            return new ContentCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        /// <summary>
        /// Gets all content categories with pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="searchTerm">Search term</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paged result of category DTOs</returns>
        public async Task<PagedResult<ContentCategoryDto>> GetAllCategoriesAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
        {
            var pagedResult = await _contentCategoryRepository.GetAllAsync(page, pageSize, searchTerm, isActive);

            var categoryDtos = pagedResult.Items.Select(category => new ContentCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            }).ToList();

            return new PagedResult<ContentCategoryDto>(categoryDtos, pagedResult.TotalItems, pagedResult.CurrentPage, pagedResult.PageSize);
        }

        /// <summary>
        /// Gets a content category by slug
        /// </summary>
        /// <param name="slug">Category slug</param>
        /// <returns>Category DTO or null if not found</returns>
        public async Task<ContentCategoryDto?> GetCategoryBySlugAsync(string slug)
        {
            var category = await _contentCategoryRepository.GetBySlugAsync(slug);
            if (category == null)
                return null;

            return new ContentCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        /// <summary>
        /// Gets all active content categories
        /// </summary>
        /// <returns>Collection of active category DTOs</returns>
        public async Task<IEnumerable<ContentCategoryDto>> GetAllActiveCategoriesAsync()
        {
            var categories = await _contentCategoryRepository.GetAllActiveAsync();

            return categories.Select(category => new ContentCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            }).ToList();
        }
    }
}