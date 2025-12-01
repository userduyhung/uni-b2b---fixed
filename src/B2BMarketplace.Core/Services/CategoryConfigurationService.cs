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
    public class CategoryConfigurationService : ICategoryConfigurationService
    {
        private readonly ICategoryConfigurationRepository _configRepository;
        private readonly IProductCategoryRepository _categoryRepository;
        private readonly ICertificationRepository _certificationRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly ILogger<CategoryConfigurationService> _logger;

        public CategoryConfigurationService(
            ICategoryConfigurationRepository configRepository,
            IProductCategoryRepository categoryRepository,
            ICertificationRepository certificationRepository,
            ISellerProfileRepository sellerProfileRepository,
            ILogger<CategoryConfigurationService> logger)
        {
            _configRepository = configRepository;
            _categoryRepository = categoryRepository;
            _certificationRepository = certificationRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _logger = logger;
        }

        public async Task<CategoryConfigurationDto?> GetCategoryConfigurationByCategoryIdAsync(Guid categoryId)
        {
            try
            {
                var configuration = await _configRepository.GetByCategoryIdAsync(categoryId);
                return configuration != null ? MapToDto(configuration) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category configuration for category ID {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<CategoryConfigurationDto?> CreateCategoryConfigurationAsync(CreateCategoryConfigurationDto configDto, Guid adminUserId)
        {
            try
            {
                // Verify the category exists
                var category = await _categoryRepository.GetByIdAsync(configDto.CategoryId);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} does not exist", configDto.CategoryId);
                    return null;
                }

                // Check if a configuration already exists for this category
                var existingConfig = await _configRepository.GetByCategoryIdAsync(configDto.CategoryId);
                if (existingConfig != null)
                {
                    _logger.LogWarning("Configuration already exists for category {CategoryId}", configDto.CategoryId);
                    return null;
                }

                var configuration = new CategoryConfiguration
                {
                    Id = Guid.NewGuid(),
                    CategoryId = configDto.CategoryId,
                    RequiredCertifications = configDto.RequiredCertifications,
                    AdditionalFields = configDto.AdditionalFields,
                    AllowsVerifiedBadge = configDto.AllowsVerifiedBadge,
                    MinCertificationsForBadge = configDto.MinCertificationsForBadge,
                    CreatedDate = DateTime.UtcNow
                };

                await _configRepository.AddAsync(configuration);
                return MapToDto(configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category configuration for category {CategoryId}", configDto.CategoryId);
                throw;
            }
        }

        public async Task<CategoryConfigurationDto?> UpdateCategoryConfigurationAsync(Guid categoryId, UpdateCategoryConfigurationDto configDto, Guid adminUserId)
        {
            try
            {
                var configuration = await _configRepository.GetByCategoryIdAsync(categoryId);
                if (configuration == null)
                {
                    return null;
                }

                configuration.RequiredCertifications = configDto.RequiredCertifications;
                configuration.AdditionalFields = configDto.AdditionalFields;
                configuration.AllowsVerifiedBadge = configDto.AllowsVerifiedBadge;
                configuration.MinCertificationsForBadge = configDto.MinCertificationsForBadge;
                configuration.UpdatedDate = DateTime.UtcNow;

                await _configRepository.UpdateAsync(configuration);
                return MapToDto(configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category configuration for category {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryConfigurationAsync(Guid categoryId)
        {
            try
            {
                var configuration = await _configRepository.GetByCategoryIdAsync(categoryId);
                if (configuration == null)
                {
                    return false;
                }

                await _configRepository.DeleteAsync(configuration.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category configuration for category {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<bool> CanSellerReceiveBadgeAsync(Guid sellerProfileId, Guid categoryId)
        {
            try
            {
                // Get the category configuration
                var configuration = await _configRepository.GetByCategoryIdAsync(categoryId);
                if (configuration == null || !configuration.AllowsVerifiedBadge)
                {
                    return false;
                }

                // Get all certifications for this seller
                var sellerCertifications = await _certificationRepository.GetBySellerProfileIdAsync(sellerProfileId);

                // Get approved certifications
                var approvedCertifications = sellerCertifications
                    .Where(c => c.Status == Enums.CertificationStatus.Approved)
                    .ToList();

                // Check if seller has enough certifications based on minimum requirement
                if (approvedCertifications.Count < configuration.MinCertificationsForBadge)
                {
                    return false;
                }

                // If there are specific required certifications defined in the configuration,
                // check if the seller has them
                if (!string.IsNullOrEmpty(configuration.RequiredCertifications))
                {
                    try
                    {
                        // Parse required certifications (stored as JSON in the RequiredCertifications field)
                        // For this basic implementation, assume it's a comma-separated list of certification names
                        var requiredCerts = configuration.RequiredCertifications
                            .Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(cert => cert.Trim())
                            .Where(cert => !string.IsNullOrEmpty(cert))
                            .ToList();

                        // For each required certification, check if the seller has an approved one
                        foreach (var requiredCert in requiredCerts)
                        {
                            var hasRequiredCert = approvedCertifications
                                .Any(cert => cert.Name.Equals(requiredCert, StringComparison.OrdinalIgnoreCase));

                            if (!hasRequiredCert)
                            {
                                return false; // Missing a required certification
                            }
                        }
                    }
                    catch
                    {
                        // If there's an error parsing the required certifications, 
                        // fall back to just checking the minimum count
                        _logger.LogWarning($"Failed to parse required certifications for category {categoryId}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if seller {SellerProfileId} can receive badge for category {CategoryId}", sellerProfileId, categoryId);
                throw;
            }
        }

        public async Task<bool> UpdateSellerVerifiedBadgeStatusAsync(Guid sellerProfileId, Guid categoryId, Guid adminUserId)
        {
            try
            {
                // Check if seller can receive badge
                var canReceiveBadge = await CanSellerReceiveBadgeAsync(sellerProfileId, categoryId);

                // Get the seller profile to update the verified badge status
                var sellerProfile = await _sellerProfileRepository.GetByIdAsync(sellerProfileId);
                if (sellerProfile == null)
                {
                    _logger.LogWarning("Seller profile with ID {SellerProfileId} not found", sellerProfileId);
                    return false;
                }

                // Update the verified badge status
                sellerProfile.HasVerifiedBadge = canReceiveBadge;
                sellerProfile.UpdatedAt = DateTime.UtcNow;

                // Save the updated seller profile
                await _sellerProfileRepository.UpdateAsync(sellerProfile);

                _logger.LogInformation("Updated verified badge status for seller {SellerProfileId} in category {CategoryId}: {Status}",
                    sellerProfileId, categoryId, canReceiveBadge ? "Granted" : "Revoked");

                return canReceiveBadge;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating verified badge status for seller {SellerProfileId} in category {CategoryId}", sellerProfileId, categoryId);
                throw;
            }
        }

        private async Task<List<Certification>> GetSellerCertificationsAsync(Guid sellerProfileId, Guid categoryId)
        {
            // Get all certifications for this seller
            var certifications = await _certificationRepository.GetBySellerProfileIdAsync(sellerProfileId);

            // In the current architecture, certifications are not directly linked to categories
            // So we simply return all certifications for this seller
            return certifications;
        }

        private CategoryConfigurationDto? MapToDto(CategoryConfiguration? config)
        {
            if (config == null) return null;

            return new CategoryConfigurationDto
            {
                Id = config.Id,
                CategoryId = config.CategoryId,
                CategoryName = config.Category?.Name ?? string.Empty, // This assumes the navigation property is loaded
                RequiredCertifications = config.RequiredCertifications,
                AdditionalFields = config.AdditionalFields,
                AllowsVerifiedBadge = config.AllowsVerifiedBadge,
                MinCertificationsForBadge = config.MinCertificationsForBadge,
                CreatedDate = config.CreatedDate,
                UpdatedDate = config.UpdatedDate
            };
        }
    }
}