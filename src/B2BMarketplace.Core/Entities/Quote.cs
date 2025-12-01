using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    public class Quote
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RFQId { get; set; }

        public Guid SellerProfileId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        public string DeliveryTime { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public DateTime ValidUntil { get; set; }
        
        public string Status { get; set; } = "Pending";

        [Required]
        public string Conditions { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("RFQId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual RFQ RFQ { get; set; } = null!;

        [ForeignKey("SellerProfileId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual SellerProfile SellerProfile { get; set; } = null!;
    }
}