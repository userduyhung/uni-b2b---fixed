using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BMarketplace.Api.Controllers.Admin
{
    /// <summary>
    /// Controller for admin category management
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin/category-management")]
    public class AdminCategoryController : ControllerBase
    {
        private readonly IAdminCategoryService _adminCategoryService;

        public AdminCategoryController(IAdminCategoryService adminCategoryService)
        {
            _adminCategoryService = adminCategoryService;
        }

        /// <summary>
        /// Gets all categories with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="size">Page size (default: 10)</param>
        /// <returns>Paginated list of categories</returns>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<CategoryDto>>> GetCategoriesAsync([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _adminCategoryService.GetCategoriesAsync(page, size);
            return Ok(result);
        }

        /// <summary>
        /// Gets a category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryByIdAsync(Guid id)
        {
            var category = await _adminCategoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="categoryDto">Category data</param>
        /// <returns>Created category</returns>
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategoryAsync([FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCategory = await _adminCategoryService.CreateCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = createdCategory.Id }, createdCategory);
        }

        /// <summary>
        /// Updates an existing category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="categoryDto">Updated category data</param>
        /// <returns>Updated category</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategoryAsync(Guid id, [FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedCategory = await _adminCategoryService.UpdateCategoryAsync(id, categoryDto);
            if (updatedCategory == null)
            {
                return NotFound();
            }
            return Ok(updatedCategory);
        }

        /// <summary>
        /// Deletes a category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(Guid id)
        {
            var result = await _adminCategoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Searches categories by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="size">Page size (default: 10)</param>
        /// <returns>Paged list of matching categories</returns>
        [HttpGet("search")]
        public async Task<ActionResult<PagedResultDto<CategoryDto>>> SearchCategoriesAsync([FromQuery] string searchTerm, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _adminCategoryService.SearchCategoriesAsync(searchTerm, page, size);
            return Ok(result);
        }
    }
}
