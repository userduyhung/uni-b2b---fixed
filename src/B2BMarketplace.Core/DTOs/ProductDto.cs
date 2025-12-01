using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for Product entity
    /// </summary>
    public class ProductDto
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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Detailed product description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Path to product image
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// Product category
        /// </summary>
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
        public int StockQuantity { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating a new Product
    /// </summary>
    public class CreateProductDto
    {
        /// <summary>
        /// Product name (required)
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Product description (optional)
        /// </summary>
        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Product category (optional)
        /// </summary>
        [StringLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Reference price (required, must be >= 0)
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal ReferencePrice { get; set; }
    }

    /// <summary>
    /// Data transfer object for updating an existing Product
    /// </summary>
    public class UpdateProductDto
    {
        /// <summary>
        /// Product name (optional)
        /// </summary>
        [StringLength(255)]
        public string? Name { get; set; }

        /// <summary>
        /// Product description (optional)
        /// </summary>
        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Product category (optional)
        /// </summary>
        [StringLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Reference price (optional, must be >= 0 if provided)
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal? ReferencePrice { get; set; }

        /// <summary>
        /// Whether product is visible to buyers (optional)
        /// </summary>
        public bool? IsActive { get; set; }
    }
}