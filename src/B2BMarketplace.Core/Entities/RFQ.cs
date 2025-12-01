using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.Entities
{
    public class RFQ
    {
        [Key]
        public Guid Id { get; set; }

        public Guid BuyerProfileId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public RFQStatus Status { get; set; } = RFQStatus.Open;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ClosedAt { get; set; }

        // Navigation properties
        [ForeignKey("BuyerProfileId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual BuyerProfile BuyerProfile { get; set; } = null!;

        public virtual List<RFQItem> Items { get; set; } = new();
        public virtual List<RFQRecipient> Recipients { get; set; } = new();
        public virtual List<Quote> Quotes { get; set; } = new();
    }
}