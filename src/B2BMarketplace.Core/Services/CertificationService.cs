using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for certification management operations
    /// </summary>
    public class CertificationService : ICertificationService
    {
        private readonly ICertificationRepository _certificationRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly INotificationService _notificationService;
        private readonly IVerificationService _verificationService;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Maximum file size for certification documents (5MB)
        /// </summary>
        public const long MaxFileSize = 5 * 1024 * 1024; // 5MB in bytes

        /// <summary>
        /// Allowed file extensions for certification documents
        /// </summary>
        public static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".jpg", ".jpeg", ".png"
        };

        /// <summary>
        /// Constructor for CertificationService
        /// </summary>
        /// <param name="certificationRepository">Certification repository</param>
        /// <param name="sellerProfileRepository">Seller profile repository</param>
        /// <param name="notificationService">Notification service</param>
        /// <param name="verificationService">Verification service</param>
        /// <param name="environment">Web host environment</param>
        public CertificationService(
            ICertificationRepository certificationRepository,
            ISellerProfileRepository sellerProfileRepository,
            INotificationService notificationService,
            IVerificationService verificationService,
            IWebHostEnvironment environment)
        {
            _certificationRepository = certificationRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _notificationService = notificationService;
            _verificationService = verificationService;
            _environment = environment;
        }

        /// <summary>
        /// Creates a new certification for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="createDto">Certification creation data</param>
        /// <returns>Created certification DTO</returns>
        public async Task<CertificationDto> CreateCertificationAsync(Guid sellerProfileId, CreateCertificationDto createDto)
        {
            // Validate file
            ValidateFile(createDto.Document);

            // Save file to disk
            var documentPath = await SaveFileAsync(createDto.Document, sellerProfileId);

            // Create certification entity
            var certification = new Certification
            {
                SellerProfileId = sellerProfileId,
                Name = createDto.Name,
                DocumentPath = documentPath,
                Status = CertificationStatus.Pending,
                SubmittedAt = DateTime.UtcNow
            };

            // Save to database
            var savedCertification = await _certificationRepository.CreateAsync(certification);

            // Update verification status (in case this new certification affects verification)
            await _verificationService.UpdateVerificationStatusAsync(sellerProfileId);

            // Return DTO
            return MapToDto(savedCertification);
        }

        /// <summary>
        /// Gets all certifications for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>List of certification DTOs</returns>
        public async Task<List<CertificationDto>> GetCertificationsBySellerAsync(Guid sellerProfileId)
        {
            var certifications = await _certificationRepository.GetBySellerProfileIdAsync(sellerProfileId);
            return certifications.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Gets all certifications with a specific status (for admin use)
        /// </summary>
        /// <param name="status">Certification status to filter by</param>
        /// <returns>List of certification DTOs</returns>
        public async Task<List<CertificationDto>> GetCertificationsByStatusAsync(CertificationStatus status)
        {
            var certifications = await _certificationRepository.GetByStatusAsync(status);
            return certifications.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Updates the status of a certification (for admin use)
        /// </summary>
        /// <param name="certificationId">Certification ID</param>
        /// <param name="updateDto">Status update data</param>
        /// <returns>Updated certification DTO</returns>
        public async Task<CertificationDto> UpdateCertificationStatusAsync(Guid certificationId, UpdateCertificationStatusDto updateDto)
        {
            // Get existing certification
            var certification = await _certificationRepository.GetByIdAsync(certificationId);
            if (certification == null)
            {
                throw new ArgumentException($"Certification with ID {certificationId} not found", nameof(certificationId));
            }

            // Validate status transition
            if (certification.Status != CertificationStatus.Pending)
            {
                throw new InvalidOperationException($"Certification status can only be updated when it is Pending. Current status: {certification.Status}");
            }

            // Update certification
            certification.Status = updateDto.Status;
            certification.AdminNotes = updateDto.AdminNotes;
            certification.ReviewedAt = DateTime.UtcNow;

            // Save to database
            var updatedCertification = await _certificationRepository.UpdateAsync(certification);

            // Send notification to seller
            await _notificationService.SendCertificationStatusNotificationAsync(updatedCertification);

            // Update verification status based on new certification status
            await _verificationService.UpdateVerificationStatusAsync(certification.SellerProfileId);

            // Return DTO
            return MapToDto(updatedCertification);
        }

        /// <summary>
        /// Gets a certification by ID
        /// </summary>
        /// <param name="certificationId">Certification ID</param>
        /// <returns>Certification DTO</returns>
        public async Task<CertificationDto?> GetCertificationByIdAsync(Guid certificationId)
        {
            var certification = await _certificationRepository.GetByIdWithSellerProfileAsync(certificationId);
            return certification != null ? MapToDto(certification) : null;
        }

        /// <summary>
        /// Validates the uploaded file
        /// </summary>
        /// <param name="file">File to validate</param>
        private void ValidateFile(IFormFile file)
        {
            // Check if file is provided
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Certification document is required", nameof(file));
            }

            // Check file size
            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException($"File size exceeds the maximum allowed size of {MaxFileSize} bytes ({MaxFileSize / (1024 * 1024)} MB)", nameof(file));
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}", nameof(file));
            }
        }

        /// <summary>
        /// Saves the uploaded file to disk
        /// </summary>
        /// <param name="file">File to save</param>
        /// <param name="sellerProfileId">Seller profile ID (used for folder organization)</param>
        /// <returns>Path to the saved file</returns>
        private async Task<string> SaveFileAsync(IFormFile file, Guid sellerProfileId)
        {
            // Create certifications directory if it doesn't exist
            var certificationsDir = Path.Combine(_environment.ContentRootPath, "wwwroot", "certifications");
            if (!Directory.Exists(certificationsDir))
            {
                Directory.CreateDirectory(certificationsDir);
            }

            // Create seller-specific directory
            var sellerDir = Path.Combine(certificationsDir, sellerProfileId.ToString());
            if (!Directory.Exists(sellerDir))
            {
                Directory.CreateDirectory(sellerDir);
            }

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(sellerDir, fileName);

            // Save file
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Return relative path for web access
            return $"/certifications/{sellerProfileId}/{fileName}";
        }

        /// <summary>
        /// Maps a Certification entity to a CertificationDto
        /// </summary>
        /// <param name="certification">Certification entity</param>
        /// <returns>Certification DTO</returns>
        private static CertificationDto MapToDto(Certification certification)
        {
            return new CertificationDto
            {
                Id = certification.Id,
                Name = certification.Name,
                Status = certification.Status,
                SubmittedAt = certification.SubmittedAt,
                ReviewedAt = certification.ReviewedAt,
                AdminNotes = certification.AdminNotes
            };
        }
    }
}