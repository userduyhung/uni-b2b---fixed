using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for managing the integration between categories and seller profiles
    /// </summary>
    public class CategorySellerProfileService : ICategorySellerProfileService
    {
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly ICertificationRepository _certificationRepository;
        private readonly ICategoryConfigurationService _categoryConfigurationService;
        private readonly ILogger<CategorySellerProfileService> _logger;

        public CategorySellerProfileService(
            ISellerProfileRepository sellerProfileRepository,
            ICertificationRepository certificationRepository,
            ICategoryConfigurationService categoryConfigurationService,
            ILogger<CategorySellerProfileService> logger)
        {
            _sellerProfileRepository = sellerProfileRepository;
            _certificationRepository = certificationRepository;
            _categoryConfigurationService = categoryConfigurationService;
            _logger = logger;
        }

        /// <summary>
        /// Updates seller profile with extended fields including business name and primary category
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="businessName">Business name</param>
        /// <param name="primaryCategoryId">Primary category ID</param>
        /// <returns>Updated seller profile</returns>
        public async Task<SellerProfileWithCertificationsDto> UpdateSellerProfileExtendedAsync(Guid sellerProfileId, string? businessName, Guid? primaryCategoryId)
        {
            var existingProfile = await _sellerProfileRepository.GetByIdAsync(sellerProfileId);
            if (existingProfile == null)
            {
                throw new ArgumentException($"Seller profile with ID {sellerProfileId} not found");
            }

            if (businessName != null)
            {
                existingProfile.BusinessName = businessName;
            }

            if (primaryCategoryId != null)
            {
                existingProfile.PrimaryCategoryId = primaryCategoryId;
            }

            existingProfile.UpdatedAt = DateTime.UtcNow;

            var updatedProfile = await _sellerProfileRepository.UpdateAsync(existingProfile);

            // Get the updated certifications
            var certifications = await _certificationRepository.GetBySellerProfileIdAsync(updatedProfile.Id);
            var approvedCertifications = certifications
                .Where(c => c.Status == CertificationStatus.Approved)
                .Select(c => new CertificationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Status = c.Status,
                    SubmittedAt = c.SubmittedAt,
                    ReviewedAt = c.ReviewedAt,
                    AdminNotes = c.AdminNotes
                })
                .ToList();

            return new SellerProfileWithCertificationsDto
            {
                Id = updatedProfile.Id,
                CompanyName = updatedProfile.CompanyName,
                BusinessName = updatedProfile.BusinessName,
                LegalRepresentative = updatedProfile.LegalRepresentative,
                TaxId = updatedProfile.TaxId,
                Industry = updatedProfile.Industry,
                Country = updatedProfile.Country,
                Description = updatedProfile.Description,
                IsVerified = updatedProfile.IsVerified,
                IsPremium = updatedProfile.IsPremium,
                HasVerifiedBadge = updatedProfile.HasVerifiedBadge,
                PrimaryCategoryId = updatedProfile.PrimaryCategoryId,
                Certifications = approvedCertifications
            };
        }

        /// <summary>
        /// Updates the verified badge status for a seller based on category configuration
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="categoryId">Category ID to check requirements for</param>
        /// <returns>True if badge status was updated successfully</returns>
        public async Task<bool> UpdateVerifiedBadgeStatusAsync(Guid sellerProfileId, Guid categoryId)
        {
            try
            {
                var canReceiveBadge = await _categoryConfigurationService.CanSellerReceiveBadgeAsync(sellerProfileId, categoryId);

                var existingProfile = await _sellerProfileRepository.GetByIdAsync(sellerProfileId);
                if (existingProfile == null)
                {
                    _logger.LogWarning("Seller profile with ID {SellerProfileId} not found when updating verified badge status", sellerProfileId);
                    return false;
                }

                existingProfile.HasVerifiedBadge = canReceiveBadge;
                existingProfile.UpdatedAt = DateTime.UtcNow;

                await _sellerProfileRepository.UpdateAsync(existingProfile);

                _logger.LogInformation("Verified badge status updated for seller profile {SellerProfileId} based on category {CategoryId}: {HasVerifiedBadge}",
                    sellerProfileId, categoryId, canReceiveBadge);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating verified badge status for seller profile {SellerProfileId} in category {CategoryId}",
                    sellerProfileId, categoryId);
                throw;
            }
        }

        /// <summary>
        /// Gets seller profile with category-specific information
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Seller profile with category information</returns>
        public async Task<SellerProfileWithCertificationsDto> GetSellerProfileWithCategoryInfoAsync(Guid sellerProfileId)
        {
            var existingProfile = await _sellerProfileRepository.GetByIdAsync(sellerProfileId);
            if (existingProfile == null)
            {
                throw new ArgumentException($"Seller profile with ID {sellerProfileId} not found");
            }

            // Get the updated certifications
            var certifications = await _certificationRepository.GetBySellerProfileIdAsync(existingProfile.Id);
            var approvedCertifications = certifications
                .Where(c => c.Status == CertificationStatus.Approved)
                .Select(c => new CertificationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Status = c.Status,
                    SubmittedAt = c.SubmittedAt,
                    ReviewedAt = c.ReviewedAt,
                    AdminNotes = c.AdminNotes
                })
                .ToList();

            return new SellerProfileWithCertificationsDto
            {
                Id = existingProfile.Id,
                CompanyName = existingProfile.CompanyName,
                BusinessName = existingProfile.BusinessName,
                LegalRepresentative = existingProfile.LegalRepresentative,
                TaxId = existingProfile.TaxId,
                Industry = existingProfile.Industry,
                Country = existingProfile.Country,
                Description = existingProfile.Description,
                IsVerified = existingProfile.IsVerified,
                IsPremium = existingProfile.IsPremium,
                HasVerifiedBadge = existingProfile.HasVerifiedBadge,
                PrimaryCategoryId = existingProfile.PrimaryCategoryId,
                Certifications = approvedCertifications
            };
        }
    }
}