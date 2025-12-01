using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a product listed by a seller
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Unique identifier for the product
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the SellerProfile entity
        /// </summary>
        public Guid SellerProfileId { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Detailed product description
        /// </summary>
        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Path to product image
        /// </summary>
        [StringLength(500)]
        public string? ImagePath { get; set; }

        /// <summary>
        /// Product category ID (foreign key to ProductCategory)
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Product category (navigation property)
        /// </summary>
        public ProductCategory? ProductCategory { get; set; }

        /// <summary>
        /// Legacy category field (for backward compatibility during migration)
        /// </summary>
        [StringLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Reference price for the product
        /// </summary>
        public decimal ReferencePrice { get; set; }

        /// <summary>
        /// Timestamp when product was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when product was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Whether product is visible to buyers
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Current stock quantity of the product
        /// </summary>
        public int StockQuantity { get; set; } = 0;

        /// <summary>
        /// Navigation property to the SellerProfile entity
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public SellerProfile SellerProfile { get; set; } = null!;

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public Product()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
            StockQuantity = 0;
        }
    }
}