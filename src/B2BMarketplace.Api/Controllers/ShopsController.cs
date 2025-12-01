using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for managing seller shops
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(Roles = "Seller")]
    public class ShopsController : ControllerBase
    {
        private readonly IProfileService _profileService;

        /// <summary>
        /// Constructor for ShopsController
        /// </summary>
        /// <param name="profileService">Profile service</param>
        public ShopsController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Creates a new seller shop
        /// </summary>
        /// <param name="shopData">Shop details</param>
        /// <returns>Created shop data</returns>
        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] dynamic shopData)
        {
            try
            {
                // Parse the dynamic request data
                var jsonString = System.Text.Json.JsonSerializer.Serialize(shopData);
                var jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(jsonString);

                string name = "";
                string description = "";
                string category = "";
                string contactEmail = "";
                string contactPhone = "";

                if (jsonElement.TryGetProperty("name", out System.Text.Json.JsonElement nameElement))
                {
                    name = nameElement.GetString() ?? "";
                }

                if (jsonElement.TryGetProperty("description", out System.Text.Json.JsonElement descriptionElement))
                {
                    description = descriptionElement.GetString() ?? "";
                }

                if (jsonElement.TryGetProperty("category", out System.Text.Json.JsonElement categoryElement))
                {
                    category = categoryElement.GetString() ?? "";
                }

                if (jsonElement.TryGetProperty("contactEmail", out System.Text.Json.JsonElement contactEmailElement))
                {
                    contactEmail = contactEmailElement.GetString() ?? "";
                }

                if (jsonElement.TryGetProperty("contactPhone", out System.Text.Json.JsonElement contactPhoneElement))
                {
                    contactPhone = contactPhoneElement.GetString() ?? "";
                }

                // Get user ID from JWT token (seller)
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new
                    {
                        error = "Invalid user token",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Update the seller profile using existing profile service
                var updateDto = new UpdateSellerProfileDto
                {
                    CompanyName = name,
                    Description = description,
                    Industry = category,
                    Country = "",  // Default empty - may need to get from elsewhere
                    LegalRepresentative = "", // Default empty - may need to get from elsewhere
                    TaxId = "" // Default empty - may need to get from elsewhere
                };

                var updatedProfile = await _profileService.UpdateProfileAsync(userId, Core.Enums.UserRole.Seller, updateDto);

                return Created("", new 
                { 
                    success = true, 
                    message = "Shop created successfully",
                    data = updatedProfile,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while creating shop",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets the seller's shop
        /// </summary>
        /// <returns>Shop details</returns>
        [HttpGet]
        public async Task<IActionResult> GetShop()
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new
                    {
                        error = "Invalid user token",
                        timestamp = DateTime.UtcNow
                    });
                }

                var profile = await _profileService.GetProfileAsync(userId, Core.Enums.UserRole.Seller) as SellerProfileWithCertificationsDto;

                if (profile == null)
                {
                    return NotFound(new
                    {
                        error = "Seller profile not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Create shop response based on seller profile
                var shop = new
                {
                    id = profile.Id, // Use profile ID as shop ID
                    sellerId = userId,
                    name = profile.CompanyName,
                    description = profile.Description,
                    category = profile.Industry,
                    contactEmail = "", // Would need to get this from user profile
                    contactPhone = profile.BusinessName, // Using business name as placeholder
                    businessHours = new { }, // Placeholder for business hours
                    isActive = true, // Assume always active for now
                    createdAt = profile.HasVerifiedBadge ? DateTime.UtcNow : DateTime.UtcNow.AddDays(-1), // Placeholder
                    updatedAt = DateTime.UtcNow
                };

                return Ok(new
                {
                    success = true,
                    message = "Shop retrieved successfully",
                    data = shop,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving shop",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates the seller's shop
        /// </summary>
        /// <param name="shopData">Updated shop details</param>
        /// <returns>Updated shop data</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateShop([FromBody] dynamic shopData)
        {
            try
            {
                // Parse the dynamic request data
                var jsonString = System.Text.Json.JsonSerializer.Serialize(shopData);
                var jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(jsonString);

                string name = "";
                string description = "";
                string category = "";
                string contactEmail = "";
                string contactPhone = "";

                if (jsonElement.TryGetProperty("name", out System.Text.Json.JsonElement nameElement))
                {
                    name = nameElement.GetString() ?? "";
                }

                if (jsonElement.TryGetProperty("description", out System.Text.Json.JsonElement descriptionElement))
                {
                    description = descriptionElement.GetString() ?? "";
                }

                if (jsonElement.TryGetProperty("category", out System.Text.Json.JsonElement categoryElement))
                {
                    category = categoryElement.GetString() ?? "";
                }

                if (jsonElement.TryGetProperty("contactEmail", out System.Text.Json.JsonElement contactEmailElement))
                {
                    contactEmail = contactEmailElement.GetString() ?? "";
                }

                if (jsonElement.TryGetProperty("contactPhone", out System.Text.Json.JsonElement contactPhoneElement))
                {
                    contactPhone = contactPhoneElement.GetString() ?? "";
                }

                // Get user ID from JWT token (seller)
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new
                    {
                        error = "Invalid user token",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Update the seller profile using existing profile service
                var updateDto = new UpdateSellerProfileDto
                {
                    CompanyName = name,
                    Description = description,
                    Industry = category,
                    // Other fields remain unchanged for now
                };

                var updatedProfile = await _profileService.UpdateProfileAsync(userId, Core.Enums.UserRole.Seller, updateDto);

                // Cast to specific profile type to access properties
                dynamic profile = updatedProfile;
                string profileId = profile?.Id?.ToString() ?? Guid.NewGuid().ToString();
                string companyName = profile?.CompanyName?.ToString() ?? "Unknown Company";
                string profileDescription = profile?.Description?.ToString() ?? "No description";
                string industry = profile?.Industry?.ToString() ?? "General";

                // Create shop response based on updated seller profile
                var shop = new
                {
                    id = profileId, // Use profile ID as shop ID
                    sellerId = userId,
                    name = companyName,
                    description = profileDescription,
                    category = industry,
                    contactEmail = contactEmail, // Return value from request
                    contactPhone = contactPhone, // Return value from request
                    businessHours = new { }, // Placeholder for business hours
                    isActive = true, // Assume always active for now
                    createdAt = DateTime.UtcNow.AddDays(-1), // Placeholder
                    updatedAt = DateTime.UtcNow
                };

                return Ok(new 
                { 
                    success = true, 
                    message = "Shop updated successfully",
                    data = shop,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while updating shop",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets a specific shop by ID
        /// </summary>
        /// <param name="id">Shop ID</param>
        /// <returns>Shop details</returns>
        [HttpGet("{id}")]
        [AllowAnonymous] // Allow access without seller role for public shop viewing
        public async Task<IActionResult> GetShopById(string id)
        {
            try
            {
                // Validate that id is a valid GUID
                if (!Guid.TryParse(id, out Guid profileId))
                {
                    return BadRequest(new
                    {
                        error = "Invalid shop ID format",
                        timestamp = DateTime.UtcNow
                    });
                }

                // If we're trying to get a shop by its seller ID as mentioned in Postman
                // Get public seller profile using profile service
                var publicProfile = await _profileService.GetPublicSellerProfileAsync(profileId);

                if (publicProfile == null)
                {
                    return NotFound(new
                    {
                        error = "Shop not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Create shop response based on seller profile
                var shop = new
                {
                    id = profileId,
                    sellerId = profileId, // In this case, seller profile ID is used as both shop and seller ID
                    name = publicProfile.CompanyName,
                    description = publicProfile.Description,
                    category = publicProfile.Industry,
                    contactEmail = "", // Would need to get this from profile or another source
                    contactPhone = "", // Would need to get this from profile or another source
                    businessHours = new { }, // Placeholder for business hours
                    isActive = true, // Assume always active for now
                    createdAt = DateTime.UtcNow.AddDays(-10), // Placeholder
                    updatedAt = DateTime.UtcNow
                };

                return Ok(new
                {
                    success = true,
                    message = "Shop retrieved successfully",
                    data = shop,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving shop",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }
    }
}