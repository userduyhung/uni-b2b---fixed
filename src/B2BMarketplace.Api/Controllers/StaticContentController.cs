using Microsoft.AspNetCore.Mvc;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Controller for static content operations (FAQ, Terms, Privacy, etc.)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StaticContentController : ControllerBase
    {
        private readonly IStaticContentService _staticContentService;

        /// <summary>
        /// Constructor for StaticContentController
        /// </summary>
        /// <param name="staticContentService">Static content service</param>
        public StaticContentController(IStaticContentService staticContentService)
        {
            _staticContentService = staticContentService;
        }

        /// <summary>
        /// Gets a published static content page by its slug
        /// </summary>
        /// <param name="slug">Page slug (e.g. 'terms', 'privacy', 'faq')</param>
        /// <returns>Requested static content page</returns>
        [HttpGet("{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStaticPage(string slug)
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

                var staticContent = await _staticContentService.GetPublishedBySlugAsync(slug);

                if (staticContent == null)
                {
                    return NotFound(new
                    {
                        error = "Static content page not found",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    message = "Static content page retrieved successfully",
                    data = new
                    {
                        Id = staticContent.Id,
                        PageSlug = staticContent.PageSlug,
                        Title = staticContent.Title,
                        Content = staticContent.Content,
                        MetaTitle = staticContent.MetaTitle,
                        MetaDescription = staticContent.MetaDescription,
                        CreatedAt = staticContent.CreatedAt,
                        UpdatedAt = staticContent.UpdatedAt
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving the static content page",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets all published static content pages
        /// </summary>
        /// <returns>List of published static content pages</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPublishedPages()
        {
            try
            {
                var pages = await _staticContentService.GetPublishedAsync();

                return Ok(new
                {
                    message = "Published static content pages retrieved successfully",
                    data = pages.Select(page => new
                    {
                        Id = page.Id,
                        PageSlug = page.PageSlug,
                        Title = page.Title,
                        MetaTitle = page.MetaTitle,
                        IsActive = page.IsActive,
                        IsPublished = page.IsPublished,
                        CreatedAt = page.CreatedAt,
                        UpdatedAt = page.UpdatedAt
                    }),
                    totalCount = pages.Count(),
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving static content pages",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets all static content pages (admin only)
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paginated list of static content pages</returns>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPagesForAdmin([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;

                var result = await _staticContentService.GetAllAsync(page, pageSize);

                return Ok(new
                {
                    message = "Static content pages retrieved successfully",
                    data = result.Items.Select(page => new
                    {
                        Id = page.Id,
                        PageSlug = page.PageSlug,
                        Title = page.Title,
                        Content = page.Content, // Note: This may contain sensitive data for admin use only
                        ContentType = page.ContentType,
                        IsActive = page.IsActive,
                        IsPublished = page.IsPublished,
                        PublishedAt = page.PublishedAt,
                        MetaTitle = page.MetaTitle,
                        MetaDescription = page.MetaDescription,
                        CreatedAt = page.CreatedAt,
                        UpdatedAt = page.UpdatedAt,
                        CreatedBy = page.CreatedBy
                    }),
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
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving static content pages",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets static content by content type (e.g., 'faq', 'tos', 'privacy')
        /// </summary>
        /// <param name="contentType">Type of content to retrieve</param>
        /// <returns>List of static content pages of specified type</returns>
        [HttpGet("type/{contentType}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByContentType(string contentType)
        {
            try
            {
                // Validate content type
                var validContentTypes = new[] { "page", "tos", "privacy", "help", "faq", "legal" };
                if (string.IsNullOrWhiteSpace(contentType) || !validContentTypes.Contains(contentType.ToLower()))
                {
                    return BadRequest(new
                    {
                        error = "Invalid content type specified",
                        timestamp = DateTime.UtcNow
                    });
                }

                var pages = await _staticContentService.GetByContentTypeAsync(contentType.ToLower());

                // Only return published pages
                var publishedPages = pages.Where(p => p.IsPublished);

                return Ok(new
                {
                    message = $"Published {contentType} pages retrieved successfully",
                    data = publishedPages.Select(page => new
                    {
                        Id = page.Id,
                        PageSlug = page.PageSlug,
                        Title = page.Title,
                        ContentType = page.ContentType,
                        MetaTitle = page.MetaTitle,
                        MetaDescription = page.MetaDescription,
                        CreatedAt = page.CreatedAt,
                        UpdatedAt = page.UpdatedAt
                    }),
                    totalCount = publishedPages.Count(),
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving static content by type",
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
