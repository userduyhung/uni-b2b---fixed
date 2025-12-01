using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BMarketplace.Api.Controllers.Admin
{
    /// <summary>
    /// Controller for admin certification management
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin/certifications")]
    public class AdminCertificationController : ControllerBase
    {
        private readonly IAdminCertificationService _adminCertificationService;

        public AdminCertificationController(IAdminCertificationService adminCertificationService)
        {
            _adminCertificationService = adminCertificationService;
        }

        /// <summary>
        /// Gets all certifications with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="size">Page size (default: 10)</param>
        /// <returns>Paged list of certifications</returns>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<CertificationDto>>> GetCertificationsAsync([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _adminCertificationService.GetCertificationsAsync(page, size);
            return Ok(result);
        }

        /// <summary>
        /// Gets a certification by ID
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <returns>Certification details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CertificationDto>> GetCertificationByIdAsync(Guid id)
        {
            var certification = await _adminCertificationService.GetCertificationByIdAsync(id);
            if (certification == null)
            {
                return NotFound();
            }
            return Ok(certification);
        }

        /// <summary>
        /// Creates a new certification
        /// </summary>
        /// <param name="certificationDto">Certification data</param>
        /// <returns>Created certification</returns>
        [HttpPost]
        public async Task<ActionResult<CertificationDto>> CreateCertificationAsync([FromBody] CertificationDto certificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCertification = await _adminCertificationService.CreateCertificationAsync(certificationDto);
            return CreatedAtAction(nameof(GetCertificationByIdAsync), new { id = createdCertification.Id }, createdCertification);
        }

        /// <summary>
        /// Updates an existing certification
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <param name="certificationDto">Updated certification data</param>
        /// <returns>Updated certification</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<CertificationDto>> UpdateCertificationAsync(Guid id, [FromBody] CertificationDto certificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedCertification = await _adminCertificationService.UpdateCertificationAsync(id, certificationDto);
            if (updatedCertification == null)
            {
                return NotFound();
            }
            return Ok(updatedCertification);
        }

        /// <summary>
        /// Deletes a certification by ID
        /// </summary>
        /// <param name="id">Certification ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificationAsync(Guid id)
        {
            var result = await _adminCertificationService.DeleteCertificationAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Searches certifications by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="size">Page size (default: 10)</param>
        /// <returns>Paged list of matching certifications</returns>
        [HttpGet("search")]
        public async Task<ActionResult<PagedResultDto<CertificationDto>>> SearchCertificationsAsync([FromQuery] string searchTerm, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _adminCertificationService.SearchCertificationsAsync(searchTerm, page, size);
            return Ok(result);
        }

        /// <summary>
        /// Approves a seller's certification
        /// </summary>
        /// <param name="id">Certification ID to approve</param>
        /// <returns>Success response</returns>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveCertificationAsync(Guid id)
        {
            var userId = Guid.Empty; // In a real implementation, this would come from the authenticated user
            var result = await _adminCertificationService.ApproveCertificationAsync(id, userId);
            if (!result)
            {
                return NotFound();
            }
            return Ok(new { message = "Certification approved successfully" });
        }

        /// <summary>
        /// Rejects a seller's certification
        /// </summary>
        /// <param name="id">Certification ID to reject</param>
        /// <param name="rejectionReason">Optional reason for rejection</param>
        /// <returns>Success response</returns>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectCertificationAsync(Guid id, [FromBody] RejectCertificationRequest request)
        {
            var userId = Guid.Empty; // In a real implementation, this would come from the authenticated user
            var result = await _adminCertificationService.RejectCertificationAsync(id, userId, request.RejectionReason);
            if (!result)
            {
                return NotFound();
            }
            return Ok(new { message = "Certification rejected successfully" });
        }
    }

    /// <summary>
    /// Request model for rejecting a certification
    /// </summary>
    public class RejectCertificationRequest
    {
        public string? RejectionReason { get; set; }
    }
}