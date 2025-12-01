using System;
using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.Entities
{
    public class ProductCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public Guid? ParentCategoryId { get; set; }

        public virtual ProductCategory? ParentCategory { get; set; }
        public virtual ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation property for products in this category
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        // Navigation property for category configurations
        public virtual ICollection<CategoryConfiguration> CategoryConfigurations { get; set; } = new List<CategoryConfiguration>();
    }
}