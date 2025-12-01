using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace B2BMarketplace.Core.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _categoryRepository;
        private readonly ILogger<ProductCategoryService> _logger;

        public ProductCategoryService(
            IProductCategoryRepository categoryRepository,
            ILogger<ProductCategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                return categories.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all categories");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetActiveAsync();
                return categories.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active categories");
                throw;
            }
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                return MapToDto(category);
            }
            catch (KeyNotFoundException)
            {
                // Re-throw as the interface expects exceptions to be propagated
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID {CategoryId}", id);
                throw;
            }
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto, Guid adminUserId)
        {
            try
            {
                // Check if category name already exists
                var existingCategory = await _categoryRepository.GetByNameAsync(categoryDto.Name);
                if (existingCategory != null)
                {
                    _logger.LogWarning("Category with name {CategoryName} already exists", categoryDto.Name);
                    throw new InvalidOperationException($"Category with name '{categoryDto.Name}' already exists");
                }

                var category = new ProductCategory
                {
                    Id = Guid.NewGuid(),
                    Name = categoryDto.Name,
                    Description = categoryDto.Description,
                    ParentCategoryId = categoryDto.ParentCategoryId,
                    DisplayOrder = categoryDto.DisplayOrder,
                    IsActive = categoryDto.IsActive,
                    CreatedBy = adminUserId,
                    CreatedDate = DateTime.UtcNow
                };

                await _categoryRepository.AddAsync(category);
                return MapToDto(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                throw;
            }
        }

        public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto categoryDto, Guid adminUserId)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {id} not found");
                }

                // Check if the new name conflicts with existing categories (excluding current category)
                var existingCategory = await _categoryRepository.GetByNameAsync(categoryDto.Name);
                if (existingCategory != null && existingCategory.Id != id)
                {
                    _logger.LogWarning("Category with name {CategoryName} already exists", categoryDto.Name);
                    throw new InvalidOperationException($"Category with name '{categoryDto.Name}' already exists");
                }

                category.Name = categoryDto.Name;
                category.Description = categoryDto.Description;
                category.ParentCategoryId = categoryDto.ParentCategoryId;
                category.DisplayOrder = categoryDto.DisplayOrder;
                category.IsActive = categoryDto.IsActive;
                category.UpdatedBy = adminUserId;
                category.UpdatedDate = DateTime.UtcNow;

                await _categoryRepository.UpdateAsync(category);
                return MapToDto(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID {CategoryId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return false;
                }

                await _categoryRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID {CategoryId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetSubcategoriesAsync(Guid parentId)
        {
            try
            {
                var categories = await _categoryRepository.GetByParentIdAsync(parentId);
                return categories.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subcategories for parent ID {ParentId}", parentId);
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetRootCategoriesAsync();
                return categories.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving root categories");
                throw;
            }
        }

        private CategoryDto MapToDto(ProductCategory category)
        {
            // Since GetByIdAsync now throws an exception when not found,
            // category should never be null at this point
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name ?? string.Empty,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                CreatedDate = category.CreatedDate,
                UpdatedDate = category.UpdatedDate
            };
        }
    }
}