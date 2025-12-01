using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Enums;
using System;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for category-based seller profile management
    /// </summary>
    [ApiController]
    [Route("api/seller/category")]
    [Authorize]
    [Produces("application/json")]
    public class CategorySellerProfileController : ControllerBase
    {
        private readonly ICategorySellerProfileService _categorySellerProfileService;
        private readonly IProfileService _profileService;

        /// <summary>
        /// Constructor for CategorySellerProfileController
        /// </summary>
        /// <param name="categorySellerProfileService">Category seller profile service</param>
        /// <param name="profileService">Profile service</param>
        public CategorySellerProfileController(
            ICategorySellerProfileService categorySellerProfileService,
            IProfileService profileService)
        {
            _categorySellerProfileService = categorySellerProfileService;
            _profileService = profileService;
        }

        /// <summary>
        /// Updates the extended seller profile information including business name and primary category
        /// </summary>
        /// <param name="request">Extended seller profile update request</param>
        /// <returns>Updated profile data</returns>
        [HttpPut("profile")]
        [Authorize(Policy = "SellerOnly")]
        public async Task<IActionResult> UpdateSellerProfileExtended([FromBody] UpdateSellerProfileExtendedRequest request)
        {
            try
            {
                // Validate input
                if (request?.ProfileData == null)
                {
                    return BadRequest(new
                    {
                        error = "Extended profile data is required",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new
                    {
                        error = "Invalid user token",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Get user role from JWT token
                var userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role);
                if (userRoleClaim == null || !Enum.TryParse<UserRole>(userRoleClaim.Value, out var userRole))
                {
                    return Unauthorized(new
                    {
                        error = "Invalid user role in token",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Validate user role
                if (userRole != UserRole.Seller)
                {
                    return Unauthorized(new
                    {
                        error = "Only sellers can update extended seller profile",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Get the seller profile ID
                var sellerProfile = await _profileService.GetProfileAsync(userId, userRole) as SellerProfileWithCertificationsDto;
                if (sellerProfile == null)
                {
                    return NotFound(new
                    {
                        error = "Seller profile not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Update extended profile information
                var updatedProfile = await _categorySellerProfileService.UpdateSellerProfileExtendedAsync(
                    sellerProfile.Id,
                    request.ProfileData.BusinessName,
                    request.ProfileData.PrimaryCategoryId);

                return Ok(new
                {
                    message = "Extended seller profile updated successfully",
                    data = updatedProfile,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while updating extended seller profile",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates verified badge status for seller based on category configuration
        /// </summary>
        /// <param name="categoryId">Category ID to check requirements for</param>
        /// <returns>Success response</returns>
        [HttpPost("badge/{categoryId:guid}/update")]
        [Authorize(Policy = "SellerOnly")]
        public async Task<IActionResult> UpdateVerifiedBadgeStatus(Guid categoryId)
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new
                    {
                        error = "Invalid user token",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Get user role from JWT token
                var userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role);
                if (userRoleClaim == null || !Enum.TryParse<UserRole>(userRoleClaim.Value, out var userRole))
                {
                    return Unauthorized(new
                    {
                        error = "Invalid user role in token",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Validate user role
                if (userRole != UserRole.Seller)
                {
                    return Unauthorized(new
                    {
                        error = "Only sellers can update verified badge status",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Get the seller profile ID
                var sellerProfile = await _profileService.GetProfileAsync(userId, userRole) as SellerProfileWithCertificationsDto;
                if (sellerProfile == null)
                {
                    return NotFound(new
                    {
                        error = "Seller profile not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Update verified badge status
                var success = await _categorySellerProfileService.UpdateVerifiedBadgeStatusAsync(
                    sellerProfile.Id,
                    categoryId);

                if (!success)
                {
                    return NotFound(new
                    {
                        error = "Unable to update verified badge status",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Verified badge status updated successfully",
                    data = new { hasVerifiedBadge = true },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while updating verified badge status",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets seller profile with category-specific information
        /// </summary>
        /// <returns>Seller profile with category information</returns>
        [HttpGet("profile")]
        [Authorize(Policy = "SellerOnly")]
        public async Task<IActionResult> GetSellerProfileWithCategoryInfo()
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new
                    {
                        error = "Invalid user token",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Get user role from JWT token
                var userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role);
                if (userRoleClaim == null || !Enum.TryParse<UserRole>(userRoleClaim.Value, out var userRole))
                {
                    return Unauthorized(new
                    {
                        error = "Invalid user role in token",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Validate user role
                if (userRole != UserRole.Seller)
                {
                    return Unauthorized(new
                    {
                        error = "Only sellers can access extended profile information",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Get the seller profile ID
                var sellerProfile = await _profileService.GetProfileAsync(userId, userRole) as SellerProfileWithCertificationsDto;
                if (sellerProfile == null)
                {
                    return NotFound(new
                    {
                        error = "Seller profile not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Get profile with category information
                var profile = await _categorySellerProfileService.GetSellerProfileWithCategoryInfoAsync(sellerProfile.Id);

                return Ok(new
                {
                    message = "Seller profile with category information retrieved successfully",
                    data = profile,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving profile with category information",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }
    }
}

/// <summary>
/// Request model for updating extended seller profile
/// </summary>
public class UpdateSellerProfileExtendedRequest
{
    /// <summary>
    /// Extended seller profile data to update
    /// </summary>
    public UpdateSellerProfileExtendedDto ProfileData { get; set; } = new UpdateSellerProfileExtendedDto();
}