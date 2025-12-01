using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;
using B2BMarketplace.Core.Extensions;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for seller verification operations
    /// </summary>
    public class VerificationService : IVerificationService
    {
        private readonly ICertificationRepository _certificationRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly INotificationService _notificationService;
        private readonly IPremiumSubscriptionService _premiumSubscriptionService;

        /// <summary>
        /// Constructor for VerificationService
        /// </summary>
        /// <param name="certificationRepository">Certification repository</param>
        /// <param name="sellerProfileRepository">Seller profile repository</param>
        /// <param name="notificationService">Notification service</param>
        /// <param name="premiumSubscriptionService">Premium subscription service</param>
        public VerificationService(
            ICertificationRepository certificationRepository,
            ISellerProfileRepository sellerProfileRepository,
            INotificationService notificationService,
            IPremiumSubscriptionService premiumSubscriptionService)
        {
            _certificationRepository = certificationRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _notificationService = notificationService;
            _premiumSubscriptionService = premiumSubscriptionService;
        }

        /// <summary>
        /// Gets pending verification requests with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 50)</param>
        /// <returns>Paged list of pending verification requests</returns>
        public async Task<PagedResultDto<PendingVerificationDto>> GetPendingVerificationsAsync(int page = 1, int pageSize = 10)
        {
            // Validate parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            // Get all pending certifications with seller profiles
            var certifications = await _certificationRepository.GetByStatusAsync(CertificationStatus.Pending);

            // Calculate pagination
            var totalItems = certifications.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var items = certifications
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new PendingVerificationDto
                {
                    Id = c.Id,
                    SellerId = c.SellerProfileId,
                    SellerName = c.SellerProfile?.User?.Email ?? "Unknown",
                    CompanyName = c.SellerProfile?.CompanyName ?? "Unknown",
                    SubmittedAt = c.SubmittedAt,
                    CertificationName = c.Name
                })
                .ToList();

            return new PagedResultDto<PendingVerificationDto>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages
            };
        }

        /// <summary>
        /// Gets detailed information for a verification request
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <returns>Verification details DTO</returns>
        public async Task<VerificationDetailsDto?> GetVerificationDetailsAsync(Guid id)
        {
            // Get certification with seller profile
            var certification = await _certificationRepository.GetByIdWithSellerProfileAsync(id);
            if (certification == null)
            {
                return null;
            }

            return new VerificationDetailsDto
            {
                Id = certification.Id,
                SellerId = certification.SellerProfileId,
                SellerName = certification.SellerProfile?.User?.Email ?? "Unknown",
                CompanyName = certification.SellerProfile?.CompanyName ?? "Unknown",
                LegalRepresentative = certification.SellerProfile?.LegalRepresentative ?? string.Empty,
                TaxId = certification.SellerProfile?.TaxId ?? string.Empty,
                Industry = certification.SellerProfile?.Industry,
                Country = certification.SellerProfile?.Country ?? string.Empty,
                Description = certification.SellerProfile?.Description,
                CertificationName = certification.Name,
                DocumentPath = certification.DocumentPath,
                SubmittedAt = certification.SubmittedAt,
                Status = certification.Status
            };
        }

        /// <summary>
        /// Approves a verification request
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <param name="adminNotes">Optional notes from admin</param>
        /// <returns>Success result</returns>
        public async Task<bool> ApproveVerificationAsync(Guid id, string? adminNotes = null)
        {
            // Get certification with seller profile
            var certification = await _certificationRepository.GetByIdWithSellerProfileAsync(id);
            if (certification == null)
            {
                return false;
            }

            // Validate status
            if (certification.Status != CertificationStatus.Pending)
            {
                throw new InvalidOperationException($"Certification status can only be updated when it is Pending. Current status: {certification.Status}");
            }

            // Update certification
            certification.Status = CertificationStatus.Approved;
            certification.AdminNotes = adminNotes;
            certification.ReviewedAt = DateTime.UtcNow;

            // Save updated certification
            await _certificationRepository.UpdateAsync(certification);

            // Check all certifications for this seller
            var allCertifications = await _certificationRepository.GetBySellerProfileIdAsync(certification.SellerProfileId);

            // Check if seller has active premium subscription
            var hasActivePremiumSubscription = await _premiumSubscriptionService.HasActivePremiumSubscriptionAsync(certification.SellerProfileId);

            // Determine if seller should be verified based on certifications and premium status
            var shouldBeVerified = certification.SellerProfile.ShouldBeVerified(allCertifications, hasActivePremiumSubscription);

            // Update seller verification status if needed
            if (certification.SellerProfile.IsVerified != shouldBeVerified)
            {
                certification.SellerProfile.IsVerified = shouldBeVerified;
                await _sellerProfileRepository.UpdateAsync(certification.SellerProfile);
            }

            // Send notification to seller
            await _notificationService.SendCertificationStatusNotificationAsync(certification);

            return true;
        }

        /// <summary>
        /// Rejects a verification request
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <param name="adminNotes">Required notes from admin</param>
        /// <returns>Success result</returns>
        public async Task<bool> RejectVerificationAsync(Guid id, string adminNotes)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(adminNotes))
            {
                throw new ArgumentException("Admin notes are required for rejection", nameof(adminNotes));
            }

            // Get certification with seller profile
            var certification = await _certificationRepository.GetByIdWithSellerProfileAsync(id);
            if (certification == null)
            {
                return false;
            }

            // Validate status
            if (certification.Status != CertificationStatus.Pending)
            {
                throw new InvalidOperationException($"Certification status can only be updated when it is Pending. Current status: {certification.Status}");
            }

            // Update certification
            certification.Status = CertificationStatus.Rejected;
            certification.AdminNotes = adminNotes;
            certification.ReviewedAt = DateTime.UtcNow;

            // Save updated certification
            await _certificationRepository.UpdateAsync(certification);

            // Check all certifications for this seller
            var allCertifications = await _certificationRepository.GetBySellerProfileIdAsync(certification.SellerProfileId);

            // Check if seller has active premium subscription
            var hasActivePremiumSubscription = await _premiumSubscriptionService.HasActivePremiumSubscriptionAsync(certification.SellerProfileId);

            // Determine if seller should be verified based on certifications and premium status
            var shouldBeVerified = certification.SellerProfile.ShouldBeVerified(allCertifications, hasActivePremiumSubscription);

            // Update seller verification status if needed
            if (certification.SellerProfile.IsVerified != shouldBeVerified)
            {
                certification.SellerProfile.IsVerified = shouldBeVerified;
                await _sellerProfileRepository.UpdateAsync(certification.SellerProfile);
            }

            // Send notification to seller
            await _notificationService.SendCertificationStatusNotificationAsync(certification);

            return true;
        }

        /// <summary>
        /// Updates verification status for a seller based on their certifications and premium status
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>True if verification status was updated, false otherwise</returns>
        public async Task<bool> UpdateVerificationStatusAsync(Guid sellerProfileId)
        {
            // Get seller profile
            var sellerProfile = await _sellerProfileRepository.GetByIdAsync(sellerProfileId);
            if (sellerProfile == null)
            {
                return false;
            }

            // Get all certifications for this seller
            var certifications = await _certificationRepository.GetBySellerProfileIdAsync(sellerProfileId);

            // Check if seller has active premium subscription
            var hasActivePremiumSubscription = await _premiumSubscriptionService.HasActivePremiumSubscriptionAsync(sellerProfileId);

            // Determine if seller should be verified based on certifications and premium status
            var shouldBeVerified = sellerProfile.ShouldBeVerified(certifications, hasActivePremiumSubscription);

            // Update seller verification status if needed
            if (sellerProfile.IsVerified != shouldBeVerified)
            {
                sellerProfile.IsVerified = shouldBeVerified;
                await _sellerProfileRepository.UpdateAsync(sellerProfile);
                return true;
            }

            return false; // No change was needed
        }

        /// <summary>
        /// Gets all sellers with their verification and premium status with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 50)</param>
        /// <param name="status">Filter by verification status (optional: verified, unverified, premium)</param>
        /// <returns>Paged list of sellers with verification and premium status</returns>
        public async Task<PagedResultDto<VerificationSummaryDto>> GetAllVerificationsAsync(int page = 1, int pageSize = 10, string? status = null)
        {
            // Validate parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            // Get sellers with pagination and filter based on status
            var allSellers = await _sellerProfileRepository.GetAllAsync();
            var sellersList = new List<SellerProfile>(allSellers);

            // Filter sellers based on status if provided
            if (!string.IsNullOrEmpty(status))
            {
                var statusLower = status.ToLower();
                switch (statusLower)
                {
                    case "verified":
                        sellersList = sellersList.Where(s => s.IsVerified).ToList();
                        break;
                    case "unverified":
                        sellersList = sellersList.Where(s => !s.IsVerified).ToList();
                        break;
                    case "premium":
                        sellersList = sellersList.Where(s => s.IsPremium).ToList();
                        break;
                }
            }

            // Calculate pagination
            var totalItems = sellersList.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Get the page of items
            var sellersPage = sellersList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Create summary DTOs for each seller
            var summaryDtos = new List<VerificationSummaryDto>();

            foreach (var seller in sellersPage)
            {
                var certifications = await _certificationRepository.GetBySellerProfileIdAsync(seller.Id);
                var approvedCertifications = certifications.Count(c => c.Status == CertificationStatus.Approved);
                var pendingCertifications = certifications.Count(c => c.Status == CertificationStatus.Pending);

                summaryDtos.Add(new VerificationSummaryDto
                {
                    Id = seller.Id,
                    CompanyName = seller.CompanyName,
                    IsVerified = seller.IsVerified,
                    IsPremium = seller.IsPremium,
                    ApprovedCertificationsCount = approvedCertifications,
                    PendingCertificationsCount = pendingCertifications,
                    UpdatedAt = seller.UpdatedAt
                });
            }

            return new PagedResultDto<VerificationSummaryDto>
            {
                Items = summaryDtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages
            };
        }

        /// <summary>
        /// Manually updates verification status for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="isVerified">Whether the seller should be marked as verified</param>
        /// <param name="adminNotes">Optional admin notes for the change</param>
        /// <returns>True if verification status was updated, false otherwise</returns>
        public async Task<bool> ManualUpdateVerificationAsync(Guid sellerProfileId, bool isVerified, string? adminNotes = null)
        {
            // Get seller profile
            var sellerProfile = await _sellerProfileRepository.GetByIdAsync(sellerProfileId);
            if (sellerProfile == null)
            {
                return false;
            }

            // Update verification status
            sellerProfile.IsVerified = isVerified;
            sellerProfile.UpdatedAt = DateTime.UtcNow;

            await _sellerProfileRepository.UpdateAsync(sellerProfile);

            return true;
        }
    }
}