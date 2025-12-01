using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for profile management operations
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly IBuyerProfileRepository _buyerProfileRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly ICertificationRepository _certificationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IReviewRepository _reviewRepository;

        /// <summary>
        /// Constructor for ProfileService
        /// </summary>
        /// <param name="buyerProfileRepository">Buyer profile repository</param>
        /// <param name="sellerProfileRepository">Seller profile repository</param>
        /// <param name="certificationRepository">Certification repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="reviewRepository">Review repository</param>
        public ProfileService(
            IBuyerProfileRepository buyerProfileRepository,
            ISellerProfileRepository sellerProfileRepository,
            ICertificationRepository certificationRepository,
            IProductRepository productRepository,
            IReviewRepository reviewRepository)
        {
            _buyerProfileRepository = buyerProfileRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _certificationRepository = certificationRepository;
            _productRepository = productRepository;
            _reviewRepository = reviewRepository;
        }

        /// <summary>
        /// Gets the profile for the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userRole">User role</param>
        /// <returns>Profile data or null if not found</returns>
        public async Task<object?> GetProfileAsync(Guid userId, UserRole userRole)
        {
            return userRole switch
            {
                UserRole.Buyer => await GetBuyerProfileAsync(userId),
                UserRole.Seller => await GetSellerProfileWithCertificationsAsync(userId),
                _ => null
            };
        }

        /// <summary>
        /// Updates the profile for the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userRole">User role</param>
        /// <param name="profileData">Profile data to update</param>
        /// <returns>Updated profile data</returns>
        public async Task<object> UpdateProfileAsync(Guid userId, UserRole userRole, object profileData)
        {
            return userRole switch
            {
                UserRole.Buyer => await UpdateBuyerProfileAsync(userId, (UpdateBuyerProfileDto)profileData),
                UserRole.Seller when profileData is UpdateSellerProfileDto sellerProfileDto => await UpdateSellerProfileAsync(userId, sellerProfileDto),
                UserRole.Seller when profileData is UpdateSellerProfileExtendedDto extendedDto => await UpdateSellerProfileExtendedAsync(userId, extendedDto),
                UserRole.Seller => await UpdateSellerProfileAsync(userId, (UpdateSellerProfileDto)profileData),
                _ => throw new InvalidOperationException($"Profile management not supported for role: {userRole}")
            };
        }

        /// <summary>
        /// Gets the public profile for a verified seller
        /// </summary>
        /// <param name="sellerId">Seller ID</param>
        /// <returns>Public seller profile data or null if not found or not publicly visible</returns>
        public async Task<PublicSellerProfileDto?> GetPublicSellerProfileAsync(Guid sellerId)
        {
            var profile = await _sellerProfileRepository.GetByIdAsync(sellerId);
            if (profile == null || !profile.IsVerified)
            {
                // Only verified profiles are publicly visible
                return null;
            }

            // Get approved certifications for public display
            var certifications = await _certificationRepository.GetBySellerProfileIdAsync(profile.Id);
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

            // Get active products for public display
            var products = await _productRepository.GetBySellerIdAsync(profile.Id);
            var activeProducts = products
                .Where(p => p.IsActive)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    SellerProfileId = p.SellerProfileId,
                    Name = p.Name,
                    Description = p.Description,
                    ImagePath = p.ImagePath,
                    Category = p.Category,
                    ReferencePrice = p.ReferencePrice,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToList();

            // Get seller rating summary
            var ratingSummary = await _sellerProfileRepository.GetSellerRatingSummaryAsync(profile.Id);

            // Get recent reviews (last 5)
            var recentReviews = await _sellerProfileRepository.GetRecentReviewsAsync(profile.Id, 5);

            return new PublicSellerProfileDto
            {
                Id = profile.Id,
                CompanyName = profile.CompanyName,
                LegalRepresentative = profile.LegalRepresentative,
                TaxId = profile.TaxId,
                Industry = profile.Industry,
                Country = profile.Country,
                Description = profile.Description,
                IsVerified = profile.IsVerified,
                IsPremium = profile.IsPremium,
                AverageRating = ratingSummary.AverageRating,
                NumberOfRatings = ratingSummary.TotalRatings,
                RecentReviews = recentReviews,
                Certifications = approvedCertifications,
                Products = activeProducts
            };
        }

        /// <summary>
        /// Gets a list of verified sellers with public profiles
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <param name="industry">Filter by industry</param>
        /// <param name="country">Filter by country</param>
        /// <returns>List of public seller profiles with pagination information</returns>
        public async Task<PublicSellerProfilesResult> GetPublicSellerProfilesAsync(int page, int pageSize, string? industry, string? country)
        {
            var (profiles, totalCount) = await _sellerProfileRepository.GetVerifiedPublicProfilesAsync(page, pageSize, industry, country);

            var publicProfiles = new List<PublicSellerProfileDto>();

            foreach (var profile in profiles)
            {
                // Get approved certifications for public display
                var certifications = await _certificationRepository.GetBySellerProfileIdAsync(profile.Id);
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

                // Get active products for public display
                var products = await _productRepository.GetBySellerIdAsync(profile.Id);
                var activeProducts = products
                    .Where(p => p.IsActive)
                    .Select(p => new ProductDto
                    {
                        Id = p.Id,
                        SellerProfileId = p.SellerProfileId,
                        Name = p.Name,
                        Description = p.Description,
                        ImagePath = p.ImagePath,
                        Category = p.Category,
                        ReferencePrice = p.ReferencePrice,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .ToList();

                // Get seller rating summary
                var ratingSummary = await _reviewRepository.GetSellerRatingSummaryAsync(profile.Id);

                // Get recent reviews (last 3)
                var recentReviews = await _reviewRepository.GetReviewsForSellerAsync(profile.Id, 1, 3);
                var recentReviewDtos = recentReviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    BuyerProfileId = r.BuyerProfileId,
                    BuyerName = r.BuyerProfile?.Name ?? "Anonymous",
                    SellerProfileId = r.SellerProfileId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    IsReported = r.IsReported
                }).ToList();

                publicProfiles.Add(new PublicSellerProfileDto
                {
                    Id = profile.Id,
                    CompanyName = profile.CompanyName,
                    LegalRepresentative = profile.LegalRepresentative,
                    TaxId = profile.TaxId,
                    Industry = profile.Industry,
                    Country = profile.Country,
                    Description = profile.Description,
                    IsVerified = profile.IsVerified,
                    IsPremium = profile.IsPremium,
                    AverageRating = ratingSummary.AverageRating,
                    NumberOfRatings = ratingSummary.TotalRatings,
                    RecentReviews = recentReviewDtos,
                    Certifications = approvedCertifications,
                    Products = activeProducts
                });
            }

            return new PublicSellerProfilesResult
            {
                Profiles = publicProfiles,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Gets the buyer profile for the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Buyer profile data or null if not found</returns>
        private async Task<BuyerProfileDto?> GetBuyerProfileAsync(Guid userId)
        {
            var profile = await _buyerProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return null;

            return new BuyerProfileDto
            {
                Name = profile.Name,
                CompanyName = profile.CompanyName,
                Country = profile.Country,
                Phone = profile.Phone
            };
        }

        /// <summary>
        /// Gets the seller profile with certifications for the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Seller profile data with certifications or null if not found</returns>
        private async Task<SellerProfileWithCertificationsDto?> GetSellerProfileWithCertificationsAsync(Guid userId)
        {
            var profile = await _sellerProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return null;

            // Get approved certifications for public display
            var certifications = await _certificationRepository.GetBySellerProfileIdAsync(profile.Id);
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
                Id = profile.Id,
                CompanyName = profile.CompanyName,
                LegalRepresentative = profile.LegalRepresentative,
                TaxId = profile.TaxId,
                Industry = profile.Industry,
                Country = profile.Country,
                Description = profile.Description,
                IsVerified = profile.IsVerified,
                IsPremium = profile.IsPremium,
                Certifications = approvedCertifications
            };
        }

        /// <summary>
        /// Updates the buyer profile for the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="profileData">Profile data to update</param>
        /// <returns>Updated buyer profile data</returns>
        private async Task<BuyerProfileDto> UpdateBuyerProfileAsync(Guid userId, UpdateBuyerProfileDto profileData)
        {
            var existingProfile = await _buyerProfileRepository.GetByUserIdAsync(userId);

            if (existingProfile == null)
            {
                // Create new profile
                var newProfile = new BuyerProfile
                {
                    UserId = userId,
                    Name = profileData.Name,
                    CompanyName = profileData.CompanyName,
                    Country = profileData.Country,
                    Phone = profileData.Phone
                };

                var createdProfile = await _buyerProfileRepository.CreateAsync(newProfile);

                return new BuyerProfileDto
                {
                    Name = createdProfile.Name,
                    CompanyName = createdProfile.CompanyName,
                    Country = createdProfile.Country,
                    Phone = createdProfile.Phone
                };
            }
            else
            {
                // Update existing profile
                existingProfile.Name = profileData.Name;
                existingProfile.CompanyName = profileData.CompanyName;
                existingProfile.Country = profileData.Country;
                existingProfile.Phone = profileData.Phone;

                var updatedProfile = await _buyerProfileRepository.UpdateAsync(existingProfile);

                return new BuyerProfileDto
                {
                    Name = updatedProfile.Name,
                    CompanyName = updatedProfile.CompanyName,
                    Country = updatedProfile.Country,
                    Phone = updatedProfile.Phone
                };
            }
        }

        /// <summary>
        /// Updates the seller profile for the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="profileData">Profile data to update</param>
        /// <returns>Updated seller profile data</returns>
        private async Task<SellerProfileDto> UpdateSellerProfileAsync(Guid userId, UpdateSellerProfileDto profileData)
        {
            var existingProfile = await _sellerProfileRepository.GetByUserIdAsync(userId);

            if (existingProfile == null)
            {
                // Create new profile
                var newProfile = new SellerProfile
                {
                    UserId = userId,
                    CompanyName = profileData.CompanyName,
                    LegalRepresentative = profileData.LegalRepresentative,
                    TaxId = profileData.TaxId,
                    Industry = profileData.Industry,
                    Country = profileData.Country,
                    Description = profileData.Description
                };

                var createdProfile = await _sellerProfileRepository.CreateAsync(newProfile);

                return new SellerProfileDto
                {
                    Id = createdProfile.Id,
                    CompanyName = createdProfile.CompanyName,
                    LegalRepresentative = createdProfile.LegalRepresentative,
                    TaxId = createdProfile.TaxId,
                    Industry = createdProfile.Industry,
                    Country = createdProfile.Country,
                    Description = createdProfile.Description,
                    IsVerified = createdProfile.IsVerified,
                    IsPremium = createdProfile.IsPremium
                };
            }
            else
            {
                // Update existing profile
                existingProfile.CompanyName = profileData.CompanyName;
                existingProfile.LegalRepresentative = profileData.LegalRepresentative;
                existingProfile.TaxId = profileData.TaxId;
                existingProfile.Industry = profileData.Industry;
                existingProfile.Country = profileData.Country;
                existingProfile.Description = profileData.Description;

                var updatedProfile = await _sellerProfileRepository.UpdateAsync(existingProfile);

                return new SellerProfileDto
                {
                    Id = updatedProfile.Id,
                    CompanyName = updatedProfile.CompanyName,
                    LegalRepresentative = updatedProfile.LegalRepresentative,
                    TaxId = updatedProfile.TaxId,
                    Industry = updatedProfile.Industry,
                    Country = updatedProfile.Country,
                    Description = updatedProfile.Description,
                    IsVerified = updatedProfile.IsVerified,
                    IsPremium = updatedProfile.IsPremium
                };
            }
        }

        /// <summary>
        /// Updates the extended seller profile for the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="extendedProfileData">Extended profile data to update</param>
        /// <returns>Updated extended seller profile data</returns>
        private async Task<SellerProfileWithCertificationsDto> UpdateSellerProfileExtendedAsync(Guid userId, UpdateSellerProfileExtendedDto extendedProfileData)
        {
            var existingProfile = await _sellerProfileRepository.GetByUserIdAsync(userId);

            if (existingProfile == null)
            {
                throw new ArgumentException("Seller profile not found");
            }

            // Update the extended fields
            if (extendedProfileData.BusinessName != null)
            {
                existingProfile.BusinessName = extendedProfileData.BusinessName;
            }

            if (extendedProfileData.PrimaryCategoryId != null)
            {
                existingProfile.PrimaryCategoryId = extendedProfileData.PrimaryCategoryId;
            }

            if (extendedProfileData.HasVerifiedBadge != null)
            {
                existingProfile.HasVerifiedBadge = extendedProfileData.HasVerifiedBadge.Value;
            }

            existingProfile.UpdatedAt = DateTime.UtcNow;

            var updatedProfile = await _sellerProfileRepository.UpdateAsync(existingProfile);

            // Get updated certifications
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
    }
}