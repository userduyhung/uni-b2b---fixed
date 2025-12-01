using Microsoft.AspNetCore.Mvc;
using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for public category operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly IProductCategoryService _productCategoryService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productCategoryService">Product category service</param>
        public CategoriesController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        /// <summary>
        /// Get all active categories (public endpoint)
        /// </summary>
        /// <returns>List of active categories</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), 200)]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _productCategoryService.GetActiveCategoriesAsync();
                return Ok(new
                {
                    data = categories,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving categories",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Get category by ID (public endpoint)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CategoryDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            try
            {
                var category = await _productCategoryService.GetCategoryByIdAsync(id);
                return Ok(new
                {
                    data = category,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    error = "Category not found",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving the category",
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
