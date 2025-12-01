using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(Guid id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto, Guid adminUserId);
        Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto categoryDto, Guid adminUserId);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<IEnumerable<CategoryDto>> GetSubcategoriesAsync(Guid parentId);
        Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();
    }
}