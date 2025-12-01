using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a configuration setting for a service tier
    /// </summary>
    public class ServiceTierConfiguration
    {
        /// <summary>
        /// Unique identifier for the service tier configuration
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the service tier this configuration belongs to
        /// </summary>
        public Guid ServiceTierId { get; set; }

        /// <summary>
        /// Navigation property to the service tier
        /// </summary>
        [ForeignKey("ServiceTierId")]
        public virtual ServiceTier ServiceTier { get; set; } = null!;

        /// <summary>
        /// Key identifier for the configuration setting
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Value for the configuration setting
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Description of the configuration setting
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Whether this configuration is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Data type of the configuration value
        /// </summary>
        [StringLength(50)]
        public string DataType { get; set; } = "string";

        /// <summary>
        /// Date and time when the configuration was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the configuration was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public ServiceTierConfiguration()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}