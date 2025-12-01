using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// DTO for creating or updating a service tier
    /// </summary>
    public class CreateUpdateServiceTierDto
    {
        /// <summary>
        /// Name of the service tier (e.g., "Free", "Premium", "Enterprise")
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the service tier
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Monthly price of the tier
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value")]
        public decimal Price { get; set; }

        /// <summary>
        /// Whether this tier is the default tier for new sellers
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Whether this tier is currently active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Features associated with this tier
        /// </summary>
        public List<ServiceTierFeatureDto> Features { get; set; } = new();
    }

    /// <summary>
    /// DTO for a service tier
    /// </summary>
    public class ServiceTierDto
    {
        /// <summary>
        /// Unique identifier for the service tier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the service tier
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the service tier
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Monthly price of the tier
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Whether this tier is the default tier for new sellers
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Whether this tier is currently active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Features associated with this tier
        /// </summary>
        public List<ServiceTierFeatureDto> Features { get; set; } = new();

        /// <summary>
        /// Date and time when the tier was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the tier was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating or updating a service tier feature
    /// </summary>
    public class CreateUpdateServiceTierFeatureDto
    {
        /// <summary>
        /// ID of the service tier this feature belongs to
        /// </summary>
        public Guid ServiceTierId { get; set; }

        /// <summary>
        /// Name of the feature
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the feature
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Whether the feature is available in this tier
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Additional value or limit associated with the feature
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Order in which the feature should be displayed
        /// </summary>
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// DTO for a service tier feature
    /// </summary>
    public class ServiceTierFeatureDto
    {
        /// <summary>
        /// Unique identifier for the service tier feature
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the service tier this feature belongs to
        /// </summary>
        public Guid ServiceTierId { get; set; }

        /// <summary>
        /// Name of the feature
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the feature
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Whether the feature is available in this tier
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Additional value or limit associated with the feature
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Order in which the feature should be displayed
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Date and time when the feature was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the feature was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}