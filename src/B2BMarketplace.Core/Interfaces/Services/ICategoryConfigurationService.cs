using System;
using System.Threading.Tasks;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface ICategoryConfigurationService
    {
        Task<CategoryConfigurationDto?> GetCategoryConfigurationByCategoryIdAsync(Guid categoryId);
        Task<CategoryConfigurationDto?> CreateCategoryConfigurationAsync(CreateCategoryConfigurationDto configDto, Guid adminUserId);
        Task<CategoryConfigurationDto?> UpdateCategoryConfigurationAsync(Guid categoryId, UpdateCategoryConfigurationDto configDto, Guid adminUserId);
        Task<bool> DeleteCategoryConfigurationAsync(Guid categoryId);
        Task<bool> CanSellerReceiveBadgeAsync(Guid sellerProfileId, Guid categoryId);
        Task<bool> UpdateSellerVerifiedBadgeStatusAsync(Guid sellerProfileId, Guid categoryId, Guid adminUserId);
    }
}