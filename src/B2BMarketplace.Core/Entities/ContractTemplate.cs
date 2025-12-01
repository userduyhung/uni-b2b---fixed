using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a reusable template for contracts between buyers and sellers
    /// </summary>
    public class ContractTemplate
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CreatedBySellerProfileId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string TemplateType { get; set; } = string.Empty; // 'standard', 'custom', 'nda', etc.

        public bool IsActive { get; set; } = true;

        public Dictionary<string, object>? CustomFields { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual SellerProfile? CreatedBySellerProfile { get; set; }
        public virtual ICollection<ContractInstance> ContractInstances { get; set; } = new List<ContractInstance>();
    }
}