using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    public class RFQItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RFQId { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Quantity { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("RFQId")]
        public virtual RFQ RFQ { get; set; } = null!;
    }
}