using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a service tier configuration (Free, Premium, etc.)
    /// </summary>
    public class ServiceTier
    {
        /// <summary>
        /// Unique identifier for the service tier
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the service tier (e.g., "Free", "Premium", "Enterprise")
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the service tier
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Monthly price of the tier
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
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
        /// Feature differences for this tier
        /// </summary>
        public List<ServiceTierFeature> Features { get; set; } = new();

        /// <summary>
        /// Date and time when the tier was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the tier was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public ServiceTier()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
            Features = new List<ServiceTierFeature>();
        }
    }
}