using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for seller verification operations (Admin only)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationService _verificationService;

        /// <summary>
        /// Constructor for VerificationController
        /// </summary>
        /// <param name="verificationService">Verification service</param>
        public VerificationController(IVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        /// <summary>
        /// Gets pending verification requests (Admin only)
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 50)</param>
        /// <returns>Paged list of pending verification requests</returns>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingVerifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;

                var result = await _verificationService.GetPendingVerificationsAsync(page, pageSize);

                return Ok(new
                {
                    message = "Pending verifications retrieved successfully",
                    data = result,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving pending verifications",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets detailed information for a verification request (Admin only)
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <returns>Verification details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVerificationDetails(Guid id)
        {
            try
            {
                // Validate input
                if (id == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        error = "Certification ID is required",
                        timestamp = DateTime.UtcNow
                    });
                }

                var result = await _verificationService.GetVerificationDetailsAsync(id);
                if (result == null)
                {
                    return NotFound(new
                    {
                        error = "Verification request not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Verification details retrieved successfully",
                    data = result,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving verification details",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Approves a verification request (Admin only)
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <param name="decisionDto">Decision data</param>
        /// <returns>Success message</returns>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveVerification(Guid id, [FromBody] VerificationDecisionDto decisionDto)
        {
            try
            {
                // Validate input
                if (id == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        error = "Certification ID is required",
                        timestamp = DateTime.UtcNow
                    });
                }

                var result = await _verificationService.ApproveVerificationAsync(id, decisionDto.AdminNotes);
                if (!result)
                {
                    return NotFound(new
                    {
                        error = "Verification request not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Verification approved successfully",
                    data = new { certificationId = id },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while approving verification",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Rejects a verification request (Admin only)
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <param name="decisionDto">Decision data</param>
        /// <returns>Success message</returns>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectVerification(Guid id, [FromBody] VerificationDecisionDto decisionDto)
        {
            try
            {
                // Validate input
                if (id == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        error = "Certification ID is required",
                        timestamp = DateTime.UtcNow
                    });
                }

                if (string.IsNullOrWhiteSpace(decisionDto.AdminNotes))
                {
                    return BadRequest(new
                    {
                        error = "Admin notes are required for rejection",
                        timestamp = DateTime.UtcNow
                    });
                }

                var result = await _verificationService.RejectVerificationAsync(id, decisionDto.AdminNotes);
                if (!result)
                {
                    return NotFound(new
                    {
                        error = "Verification request not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Verification rejected successfully",
                    data = new { certificationId = id },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while rejecting verification",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets all sellers with their verification and premium status (Admin only)
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 50)</param>
        /// <param name="status">Filter by verification status (optional: verified, unverified, premium)</param>
        /// <returns>Paged list of sellers with verification and premium status</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllVerifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null)
        {
            try
            {
                // Validate parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;

                var result = await _verificationService.GetAllVerificationsAsync(page, pageSize, status);

                return Ok(new
                {
                    message = "Verifications retrieved successfully",
                    data = result,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving verifications",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Manually updates verification status for a seller (Admin only)
        /// </summary>
        /// <param name="id">Seller profile ID</param>
        /// <param name="manualVerificationDto">Manual verification data</param>
        /// <returns>Success message</returns>
        [HttpPost("{id}/manual-verify")]
        public async Task<IActionResult> ManualUpdateVerification(Guid id, [FromBody] ManualVerificationDto manualVerificationDto)
        {
            try
            {
                // Validate input
                if (id == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        error = "Seller profile ID is required",
                        timestamp = DateTime.UtcNow
                    });
                }

                var result = await _verificationService.ManualUpdateVerificationAsync(id, manualVerificationDto.IsVerified, manualVerificationDto.AdminNotes);
                if (!result)
                {
                    return NotFound(new
                    {
                        error = "Seller profile not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = $"Seller verification status updated to {(manualVerificationDto.IsVerified ? "verified" : "unverified")} successfully",
                    data = new { sellerId = id, isVerified = manualVerificationDto.IsVerified },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while updating verification status",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }
    }
}