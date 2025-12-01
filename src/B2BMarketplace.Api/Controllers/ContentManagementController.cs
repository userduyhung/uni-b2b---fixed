using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for admin content management operations
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class ContentManagementController : ControllerBase
    {
        private readonly IContentCategoryService _contentCategoryService;
        private readonly IContentItemService _contentItemService;
        private readonly IContentTagService _contentTagService;

        /// <summary>
        /// Constructor for ContentManagementController
        /// </summary>
        /// <param name="contentCategoryService">Content category service</param>
        /// <param name="contentItemService">Content item service</param>
        /// <param name="contentTagService">Content tag service</param>
        public ContentManagementController(
            IContentCategoryService contentCategoryService,
            IContentItemService contentItemService,
            IContentTagService contentTagService)
        {
            _contentCategoryService = contentCategoryService;
            _contentItemService = contentItemService;
            _contentTagService = contentTagService;
        }

        #region Content Category Endpoints

        /// <summary>
        /// Creates a new content category
        /// </summary>
        /// <param name="createCategoryDto">Category creation data</param>
        /// <returns>Created category</returns>
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateContentCategoryDto createCategoryDto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        error = "Invalid input data",
                        timestamp = DateTime.UtcNow
                    });
                }

                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new
                    {
                        error = "Unable to identify user",
                        timestamp = DateTime.UtcNow
                    });
                }

                var category = await _contentCategoryService.CreateCategoryAsync(createCategoryDto, userId.ToString());

                return Ok(new
                {
                    message = "Content category created successfully",
                    data = category,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while creating the content category",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates an existing content category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="updateCategoryDto">Category update data</param>
        /// <returns>Updated category</returns>
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateContentCategoryDto updateCategoryDto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        error = "Invalid input data",
                        timestamp = DateTime.UtcNow
                    });
                }

                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new
                    {
                        error = "Unable to identify user",
                        timestamp = DateTime.UtcNow
                    });
                }

                var category = await _contentCategoryService.UpdateCategoryAsync(id, updateCategoryDto, userId.ToString());

                return Ok(new
                {
                    message = "Content category updated successfully",
                    data = category,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new
                {
                    error = "Content category not found",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while updating the content category",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Deletes a content category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var success = await _contentCategoryService.DeleteCategoryAsync(id);

                if (!success)
                {
                    return NotFound(new
                    {
                        error = "Content category not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Content category deleted successfully",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while deleting the content category",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets a content category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            try
            {
                var category = await _contentCategoryService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    return NotFound(new
                    {
                        error = "Content category not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Content category retrieved successfully",
                    data = category,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving the content category",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets all content categories with pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="searchTerm">Search term</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of categories</returns>
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                // Validate parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;

                var result = await _contentCategoryService.GetAllCategoriesAsync(page, pageSize, searchTerm, isActive);

                return Ok(new
                {
                    message = "Content categories retrieved successfully",
                    data = result.Items,
                    pagination = new
                    {
                        currentPage = result.CurrentPage,
                        pageSize = result.PageSize,
                        totalCount = result.TotalItems,
                        totalPages = result.TotalPages,
                        hasPreviousPage = result.HasPreviousPage,
                        hasNextPage = result.HasNextPage
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving content categories",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        #endregion

        #region Content Item Endpoints

        /// <summary>
        /// Creates a new content item
        /// </summary>
        /// <param name="createContentItemDto">Content item creation data</param>
        /// <returns>Created content item</returns>
        [HttpPost("items")]
        public async Task<IActionResult> CreateContentItem([FromBody] CreateContentItemDto createContentItemDto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        error = "Invalid input data",
                        timestamp = DateTime.UtcNow
                    });
                }

                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new
                    {
                        error = "Unable to identify user",
                        timestamp = DateTime.UtcNow
                    });
                }

                var contentItem = await _contentItemService.CreateContentItemAsync(createContentItemDto, userId.ToString());

                return Ok(new
                {
                    message = "Content item created successfully",
                    data = contentItem,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while creating the content item",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates an existing content item
        /// </summary>
        /// <param name="id">Content item ID</param>
        /// <param name="updateContentItemDto">Content item update data</param>
        /// <returns>Updated content item</returns>
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateContentItem(Guid id, [FromBody] UpdateContentItemDto updateContentItemDto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        error = "Invalid input data",
                        timestamp = DateTime.UtcNow
                    });
                }

                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new
                    {
                        error = "Unable to identify user",
                        timestamp = DateTime.UtcNow
                    });
                }

                var contentItem = await _contentItemService.UpdateContentItemAsync(id, updateContentItemDto, userId.ToString());

                return Ok(new
                {
                    message = "Content item updated successfully",
                    data = contentItem,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new
                {
                    error = "Content item not found",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while updating the content item",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Deletes a content item
        /// </summary>
        /// <param name="id">Content item ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteContentItem(Guid id)
        {
            try
            {
                var success = await _contentItemService.DeleteContentItemAsync(id);

                if (!success)
                {
                    return NotFound(new
                    {
                        error = "Content item not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Content item deleted successfully",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while deleting the content item",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets a content item by ID
        /// </summary>
        /// <param name="id">Content item ID</param>
        /// <returns>Content item details</returns>
        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetContentItem(Guid id)
        {
            try
            {
                var contentItem = await _contentItemService.GetContentItemByIdAsync(id);

                if (contentItem == null)
                {
                    return NotFound(new
                    {
                        error = "Content item not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Content item retrieved successfully",
                    data = contentItem,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving the content item",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets all content items with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        /// <returns>Paginated list of content items</returns>
        [HttpGet("items")]
        public async Task<IActionResult> GetAllContentItems([FromQuery] ContentItemFilterDto filter)
        {
            try
            {
                // Validate parameters
                if (filter.Page < 1) filter.Page = 1;
                if (filter.PageSize < 1) filter.PageSize = 10;
                if (filter.PageSize > 50) filter.PageSize = 50;

                var result = await _contentItemService.GetAllContentItemsAsync(filter);

                return Ok(new
                {
                    message = "Content items retrieved successfully",
                    data = result.Items,
                    pagination = new
                    {
                        currentPage = result.CurrentPage,
                        pageSize = result.PageSize,
                        totalCount = result.TotalItems,
                        totalPages = result.TotalPages,
                        hasPreviousPage = result.HasPreviousPage,
                        hasNextPage = result.HasNextPage
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving content items",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Publishes a content item
        /// </summary>
        /// <param name="id">Content item ID</param>
        /// <returns>Success message</returns>
        [HttpPost("items/{id}/publish")]
        public async Task<IActionResult> PublishContentItem(Guid id)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new
                    {
                        error = "Unable to identify user",
                        timestamp = DateTime.UtcNow
                    });
                }

                var success = await _contentItemService.PublishContentItemAsync(id, userId.ToString());

                if (!success)
                {
                    return NotFound(new
                    {
                        error = "Content item not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Content item published successfully",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while publishing the content item",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Unpublishes a content item
        /// </summary>
        /// <param name="id">Content item ID</param>
        /// <returns>Success message</returns>
        [HttpPost("items/{id}/unpublish")]
        public async Task<IActionResult> UnpublishContentItem(Guid id)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new
                    {
                        error = "Unable to identify user",
                        timestamp = DateTime.UtcNow
                    });
                }

                var success = await _contentItemService.UnpublishContentItemAsync(id, userId.ToString());

                if (!success)
                {
                    return NotFound(new
                    {
                        error = "Content item not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Content item unpublished successfully",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while unpublishing the content item",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        #endregion

        #region Content Tag Endpoints

        /// <summary>
        /// Creates a new content tag
        /// </summary>
        /// <param name="createTagDto">Tag creation data</param>
        /// <returns>Created tag</returns>
        [HttpPost("tags")]
        public async Task<IActionResult> CreateTag([FromBody] CreateContentTagDto createTagDto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        error = "Invalid input data",
                        timestamp = DateTime.UtcNow
                    });
                }

                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new
                    {
                        error = "Unable to identify user",
                        timestamp = DateTime.UtcNow
                    });
                }

                var tag = await _contentTagService.CreateTagAsync(createTagDto, userId.ToString());

                return Ok(new
                {
                    message = "Content tag created successfully",
                    data = tag,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while creating the content tag",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates an existing content tag
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <param name="updateTagDto">Tag update data</param>
        /// <returns>Updated tag</returns>
        [HttpPut("tags/{id}")]
        public async Task<IActionResult> UpdateTag(Guid id, [FromBody] UpdateContentTagDto updateTagDto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        error = "Invalid input data",
                        timestamp = DateTime.UtcNow
                    });
                }

                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new
                    {
                        error = "Unable to identify user",
                        timestamp = DateTime.UtcNow
                    });
                }

                var tag = await _contentTagService.UpdateTagAsync(id, updateTagDto, userId.ToString());

                return Ok(new
                {
                    message = "Content tag updated successfully",
                    data = tag,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new
                {
                    error = "Content tag not found",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while updating the content tag",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Deletes a content tag
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("tags/{id}")]
        public async Task<IActionResult> DeleteTag(Guid id)
        {
            try
            {
                var success = await _contentTagService.DeleteTagAsync(id);

                if (!success)
                {
                    return NotFound(new
                    {
                        error = "Content tag not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Content tag deleted successfully",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while deleting the content tag",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets a content tag by ID
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <returns>Tag details</returns>
        [HttpGet("tags/{id}")]
        public async Task<IActionResult> GetTag(Guid id)
        {
            try
            {
                var tag = await _contentTagService.GetTagByIdAsync(id);

                if (tag == null)
                {
                    return NotFound(new
                    {
                        error = "Content tag not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Content tag retrieved successfully",
                    data = tag,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving the content tag",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets all content tags with pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="searchTerm">Search term</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of tags</returns>
        [HttpGet("tags")]
        public async Task<IActionResult> GetAllTags(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                // Validate parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;

                var result = await _contentTagService.GetAllTagsAsync(page, pageSize, searchTerm, isActive);

                return Ok(new
                {
                    message = "Content tags retrieved successfully",
                    data = result.Items,
                    pagination = new
                    {
                        currentPage = result.CurrentPage,
                        pageSize = result.PageSize,
                        totalCount = result.TotalItems,
                        totalPages = result.TotalPages,
                        hasPreviousPage = result.HasPreviousPage,
                        hasNextPage = result.HasNextPage
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving content tags",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Adds a tag to a content item
        /// </summary>
        /// <param name="contentItemId">Content item ID</param>
        /// <param name="tagId">Tag ID</param>
        /// <returns>Success message</returns>
        [HttpPost("items/{contentItemId}/tags/{tagId}")]
        public async Task<IActionResult> AddTagToContentItem(Guid contentItemId, Guid tagId)
        {
            try
            {
                var success = await _contentTagService.AddTagToContentItemAsync(contentItemId, tagId);

                if (!success)
                {
                    return NotFound(new
                    {
                        error = "Content item or tag not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Tag added to content item successfully",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while adding the tag to the content item",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Removes a tag from a content item
        /// </summary>
        /// <param name="contentItemId">Content item ID</param>
        /// <param name="tagId">Tag ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("items/{contentItemId}/tags/{tagId}")]
        public async Task<IActionResult> RemoveTagFromContentItem(Guid contentItemId, Guid tagId)
        {
            try
            {
                var success = await _contentTagService.RemoveTagFromContentItemAsync(contentItemId, tagId);

                if (!success)
                {
                    return NotFound(new
                    {
                        error = "Content item or tag not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Tag removed from content item successfully",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while removing the tag from the content item",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Helper method to extract user ID from JWT claims
        /// </summary>
        /// <returns>User ID or Guid.Empty if not found</returns>
        private Guid GetUserIdFromClaims()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            return Guid.Empty;
        }

        #endregion
    }
}