using System.ComponentModel.DataAnnotations;
using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for ContentCategory entity
    /// </summary>
    public class ContentCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Slug { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating a new ContentCategory
    /// </summary>
    public class CreateContentCategoryDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Slug { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Data transfer object for updating a ContentCategory
    /// </summary>
    public class UpdateContentCategoryDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Slug { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Data transfer object for ContentItem entity
    /// </summary>
    public class ContentItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string ContentType { get; set; } = "page";
        public Guid? CategoryId { get; set; }
        public ContentCategoryDto? Category { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? ScheduledPublishAt { get; set; }
        public DateTime? ScheduledUnpublishAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public List<ContentTagDto> Tags { get; set; } = new List<ContentTagDto>();
    }

    /// <summary>
    /// Data transfer object for creating a new ContentItem
    /// </summary>
    public class CreateContentItemDto
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Excerpt { get; set; }

        [StringLength(255)]
        public string? MetaTitle { get; set; }

        public string? MetaDescription { get; set; }

        [StringLength(50)]
        public string ContentType { get; set; } = "page";

        public Guid? CategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsPublished { get; set; } = false;

        public DateTime? PublishedAt { get; set; }

        public DateTime? ScheduledPublishAt { get; set; }

        public DateTime? ScheduledUnpublishAt { get; set; }

        public List<Guid> TagIds { get; set; } = new List<Guid>();
    }

    /// <summary>
    /// Data transfer object for updating a ContentItem
    /// </summary>
    public class UpdateContentItemDto
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Excerpt { get; set; }

        [StringLength(255)]
        public string? MetaTitle { get; set; }

        public string? MetaDescription { get; set; }

        [StringLength(50)]
        public string ContentType { get; set; } = "page";

        public Guid? CategoryId { get; set; }

        public bool IsActive { get; set; }

        public bool IsPublished { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? ScheduledPublishAt { get; set; }

        public DateTime? ScheduledUnpublishAt { get; set; }

        public List<Guid> TagIds { get; set; } = new List<Guid>();
    }

    /// <summary>
    /// Data transfer object for ContentTag entity
    /// </summary>
    public class ContentTagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating a new ContentTag
    /// </summary>
    public class CreateContentTagDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Data transfer object for updating a ContentTag
    /// </summary>
    public class UpdateContentTagDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Data transfer object for filtering ContentItems
    /// </summary>
    public class ContentItemFilterDto
    {
        public string? SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public string? ContentType { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsPublished { get; set; }
        public Guid? TagId { get; set; }
        public DateTime? PublishedAfter { get; set; }
        public DateTime? PublishedBefore { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortOrder { get; set; } = "desc";
    }
}