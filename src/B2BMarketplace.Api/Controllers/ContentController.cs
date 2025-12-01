using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for public content retrieval operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ContentController : ControllerBase
    {
        private readonly IContentItemService _contentItemService;
        private readonly IContentCategoryService _contentCategoryService;
        private readonly IContentTagService _contentTagService;

        /// <summary>
        /// Constructor for ContentController
        /// </summary>
        /// <param name="contentItemService">Content item service</param>
        /// <param name="contentCategoryService">Content category service</param>
        /// <param name="contentTagService">Content tag service</param>
        public ContentController(
            IContentItemService contentItemService,
            IContentCategoryService contentCategoryService,
            IContentTagService contentTagService)
        {
            _contentItemService = contentItemService;
            _contentCategoryService = contentCategoryService;
            _contentTagService = contentTagService;
        }

        /// <summary>
        /// Gets a published content item by slug
        /// </summary>
        /// <param name="slug">Content item slug</param>
        /// <returns>Published content item</returns>
        [HttpGet("{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetContentItemBySlug(string slug)
        {
            try
            {
                // Sanitize the slug to prevent path traversal
                if (string.IsNullOrWhiteSpace(slug) || slug.Contains("..") || slug.Contains("/"))
                {
                    return BadRequest(new
                    {
                        error = "Invalid slug parameter",
                        timestamp = DateTime.UtcNow
                    });
                }

                var contentItem = await _contentItemService.GetContentItemBySlugAsync(slug);

                if (contentItem == null || !contentItem.IsPublished || !contentItem.IsActive)
                {
                    return NotFound(new
                    {
                        error = "Content item not found or not published",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Content item retrieved successfully",
                    data = new
                    {
                        Id = contentItem.Id,
                        Title = contentItem.Title,
                        Content = contentItem.Content,
                        Excerpt = contentItem.Excerpt,
                        MetaTitle = contentItem.MetaTitle,
                        MetaDescription = contentItem.MetaDescription,
                        ContentType = contentItem.ContentType,
                        Category = contentItem.Category != null ? new
                        {
                            Id = contentItem.Category.Id,
                            Name = contentItem.Category.Name,
                            Slug = contentItem.Category.Slug
                        } : null,
                        Tags = contentItem.Tags.Select(t => new
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Slug = t.Slug
                        }).ToList(),
                        PublishedAt = contentItem.PublishedAt,
                        CreatedAt = contentItem.CreatedAt,
                        UpdatedAt = contentItem.UpdatedAt
                    },
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
        /// Gets all published content items with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        /// <returns>Paginated list of published content items</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPublishedContentItems([FromQuery] ContentItemFilterDto filter)
        {
            try
            {
                // Ensure only published and active items are returned
                filter.IsPublished = true;
                filter.IsActive = true;

                // Validate parameters
                if (filter.Page < 1) filter.Page = 1;
                if (filter.PageSize < 1) filter.PageSize = 10;
                if (filter.PageSize > 50) filter.PageSize = 50;

                var result = await _contentItemService.GetAllContentItemsAsync(filter);

                // Filter to only include published and active items
                var publishedItems = result.Items
                    .Where(item => item.IsPublished && item.IsActive)
                    .Select(item => new
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Excerpt = item.Excerpt,
                        Slug = item.Slug,
                        ContentType = item.ContentType,
                        Category = item.Category != null ? new
                        {
                            Id = item.Category.Id,
                            Name = item.Category.Name,
                            Slug = item.Category.Slug
                        } : null,
                        Tags = item.Tags.Select(t => new
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Slug = t.Slug
                        }).ToList(),
                        PublishedAt = item.PublishedAt,
                        CreatedAt = item.CreatedAt,
                        UpdatedAt = item.UpdatedAt
                    })
                    .ToList();

                return Ok(new
                {
                    message = "Published content items retrieved successfully",
                    data = publishedItems,
                    pagination = new
                    {
                        currentPage = result.CurrentPage,
                        pageSize = result.PageSize,
                        totalCount = publishedItems.Count,
                        totalPages = (int)Math.Ceiling((double)publishedItems.Count / filter.PageSize),
                        hasPreviousPage = result.CurrentPage > 1,
                        hasNextPage = result.CurrentPage < (int)Math.Ceiling((double)publishedItems.Count / filter.PageSize)
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving published content items",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets all published content items by category slug
        /// </summary>
        /// <param name="categorySlug">Category slug</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of published content items in the category</returns>
        [HttpGet("category/{categorySlug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetContentItemsByCategory(
            string categorySlug,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;

                // Get category by slug
                var category = await _contentCategoryService.GetCategoryBySlugAsync(categorySlug);
                if (category == null)
                {
                    return NotFound(new
                    {
                        error = "Category not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Get published content items by category
                var contentItems = await _contentItemService.GetPublishedContentItemsByCategoryAsync(category.Id);

                // Apply pagination
                var totalCount = contentItems.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var items = contentItems
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(item => new
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Excerpt = item.Excerpt,
                        Slug = item.Slug,
                        ContentType = item.ContentType,
                        Tags = item.Tags.Select(t => new
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Slug = t.Slug
                        }).ToList(),
                        PublishedAt = item.PublishedAt,
                        CreatedAt = item.CreatedAt,
                        UpdatedAt = item.UpdatedAt
                    })
                    .ToList();

                return Ok(new
                {
                    message = "Published content items retrieved successfully",
                    data = items,
                    category = new
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Slug = category.Slug,
                        Description = category.Description
                    },
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = totalPages,
                        hasPreviousPage = page > 1,
                        hasNextPage = page < totalPages
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving content items by category",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets all active categories
        /// </summary>
        /// <returns>List of active categories</returns>
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllActiveCategories()
        {
            try
            {
                var categories = await _contentCategoryService.GetAllActiveCategoriesAsync();

                var categoryDtos = categories.Select(category => new
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    Description = category.Description
                }).ToList();

                return Ok(new
                {
                    message = "Active categories retrieved successfully",
                    data = categoryDtos,
                    totalCount = categoryDtos.Count,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving categories",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets all active tags
        /// </summary>
        /// <returns>List of active tags</returns>
        [HttpGet("tags")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllActiveTags()
        {
            try
            {
                var tags = await _contentTagService.GetAllActiveTagsAsync();

                var tagDtos = tags.Select(tag => new
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Slug = tag.Slug,
                    Description = tag.Description
                }).ToList();

                return Ok(new
                {
                    message = "Active tags retrieved successfully",
                    data = tagDtos,
                    totalCount = tagDtos.Count,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving tags",
                    timestamp = DateTime.UtcNow,
                    details = ex.Message
                });
            }
        }
    }
}