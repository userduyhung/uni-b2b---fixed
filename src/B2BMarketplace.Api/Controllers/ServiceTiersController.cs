using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for managing service tiers
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceTiersController : ControllerBase
    {
        private readonly IServiceTierService _serviceTierService;

        /// <summary>
        /// Constructor for ServiceTiersController
        /// </summary>
        /// <param name="serviceTierService">Service tier service</param>
        public ServiceTiersController(IServiceTierService serviceTierService)
        {
            _serviceTierService = serviceTierService;
        }

        /// <summary>
        /// Gets all service tiers
        /// </summary>
        /// <returns>List of service tiers</returns>
        [HttpGet]
        [AllowAnonymous] // Allow anonymous access so sellers can view available tiers
        public async Task<ActionResult<IEnumerable<ServiceTierDto>>> GetServiceTiers()
        {
            var serviceTiers = await _serviceTierService.GetServiceTiersAsync();
            return Ok(serviceTiers);
        }

        /// <summary>
        /// Gets a service tier by ID
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>Service tier details</returns>
        [HttpGet("{id}")]
        [AllowAnonymous] // Allow anonymous access so sellers can view specific tier
        public async Task<ActionResult<ServiceTierDto>> GetServiceTier(Guid id)
        {
            var serviceTier = await _serviceTierService.GetServiceTierByIdAsync(id);
            if (serviceTier == null)
            {
                return NotFound($"Service tier with ID {id} not found");
            }

            return Ok(serviceTier);
        }

        /// <summary>
        /// Creates a new service tier
        /// </summary>
        /// <param name="createServiceTierDto">Service tier creation parameters</param>
        /// <returns>Created service tier</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ServiceTierDto>> CreateServiceTier([FromBody] CreateUpdateServiceTierDto createServiceTierDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdServiceTier = await _serviceTierService.CreateServiceTierAsync(createServiceTierDto);
            return CreatedAtAction(nameof(GetServiceTier), new { id = createdServiceTier.Id }, createdServiceTier);
        }

        /// <summary>
        /// Updates an existing service tier
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <param name="updateServiceTierDto">Service tier update parameters</param>
        /// <returns>Updated service tier</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ServiceTierDto>> UpdateServiceTier(Guid id, [FromBody] CreateUpdateServiceTierDto updateServiceTierDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedServiceTier = await _serviceTierService.UpdateServiceTierAsync(id, updateServiceTierDto);
                return Ok(updatedServiceTier);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a service tier
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>Success or error response</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteServiceTier(Guid id)
        {
            var result = await _serviceTierService.DeleteServiceTierAsync(id);
            if (!result)
            {
                return NotFound($"Service tier with ID {id} not found");
            }

            return NoContent();
        }

        /// <summary>
        /// Gets the default service tier
        /// </summary>
        /// <returns>Default service tier</returns>
        [HttpGet("default")]
        [AllowAnonymous] // Allow anonymous access so sellers can see default tier
        public async Task<ActionResult<ServiceTierDto>> GetDefaultServiceTier()
        {
            var defaultServiceTier = await _serviceTierService.GetDefaultServiceTierAsync();
            if (defaultServiceTier == null)
            {
                return NotFound("No default service tier found");
            }

            return Ok(defaultServiceTier);
        }

        /// <summary>
        /// Gets a service tier by name
        /// </summary>
        /// <param name="name">Service tier name</param>
        /// <returns>Service tier details</returns>
        [HttpGet("name/{name}")]
        [AllowAnonymous] // Allow anonymous access so sellers can find tier by name
        public async Task<ActionResult<ServiceTierDto>> GetServiceTierByName([FromRoute] string name)
        {
            var serviceTier = await _serviceTierService.GetServiceTierByNameAsync(name);
            if (serviceTier == null)
            {
                return NotFound($"Service tier with name '{name}' not found");
            }

            return Ok(serviceTier);
        }

        #region Feature Management Endpoints

        /// <summary>
        /// Adds a feature to a service tier
        /// </summary>
        /// <param name="createFeatureDto">Feature creation parameters</param>
        /// <returns>Created feature</returns>
        [HttpPost("features")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ServiceTierFeatureDto>> AddFeatureToServiceTier([FromBody] CreateUpdateServiceTierFeatureDto createFeatureDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addedFeature = await _serviceTierService.AddFeatureToServiceTierAsync(createFeatureDto);
            return CreatedAtAction(nameof(GetFeatureById), new { id = addedFeature.Id }, addedFeature);
        }

        /// <summary>
        /// Updates a feature in a service tier
        /// </summary>
        /// <param name="id">Feature ID</param>
        /// <param name="updateFeatureDto">Feature update parameters</param>
        /// <returns>Updated feature</returns>
        [HttpPut("features/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ServiceTierFeatureDto>> UpdateFeatureInServiceTier(Guid id, [FromBody] CreateUpdateServiceTierFeatureDto updateFeatureDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedFeature = await _serviceTierService.UpdateFeatureInServiceTierAsync(id, updateFeatureDto);
                return Ok(updatedFeature);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a feature from a service tier
        /// </summary>
        /// <param name="id">Feature ID</param>
        /// <returns>Success or error response</returns>
        [HttpDelete("features/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteFeatureFromServiceTier(Guid id)
        {
            var result = await _serviceTierService.DeleteFeatureFromServiceTierAsync(id);
            if (!result)
            {
                return NotFound($"Feature with ID {id} not found");
            }

            return NoContent();
        }

        /// <summary>
        /// Gets a feature by ID
        /// </summary>
        /// <param name="id">Feature ID</param>
        /// <returns>Feature details</returns>
        [HttpGet("features/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ServiceTierFeatureDto>> GetFeatureById(Guid id)
        {
            var feature = await _serviceTierService.GetFeatureByIdAsync(id);
            if (feature == null)
            {
                return NotFound($"Feature with ID {id} not found");
            }

            return Ok(feature);
        }

        /// <summary>
        /// Gets features for a specific service tier
        /// </summary>
        /// <param name="serviceTierId">Service tier ID</param>
        /// <returns>List of features for the service tier</returns>
        [HttpGet("{serviceTierId}/features")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<ServiceTierFeatureDto>>> GetFeaturesByServiceTierId(Guid serviceTierId)
        {
            var features = await _serviceTierService.GetFeaturesByServiceTierIdAsync(serviceTierId);
            return Ok(features);
        }

        #endregion
    }
}