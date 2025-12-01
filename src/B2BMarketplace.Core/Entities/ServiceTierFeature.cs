using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a feature associated with a service tier
    /// </summary>
    public class ServiceTierFeature
    {
        /// <summary>
        /// Unique identifier for the service tier feature
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the service tier this feature belongs to
        /// </summary>
        public Guid ServiceTierId { get; set; }

        /// <summary>
        /// Navigation property to the service tier
        /// </summary>
        [ForeignKey("ServiceTierId")]
        public virtual ServiceTier ServiceTier { get; set; } = null!;

        /// <summary>
        /// Name of the feature
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the feature
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Whether the feature is available in this tier
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Additional value or limit associated with the feature (e.g., number of products, storage limit)
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

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public ServiceTierFeature()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}