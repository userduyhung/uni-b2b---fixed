using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a specific instance of a contract generated from a template
    /// </summary>
    public class ContractInstance
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ContractTemplateId { get; set; }

        [Required]
        public Guid BuyerProfileId { get; set; }

        [Required]
        public Guid SellerProfileId { get; set; }

        public Guid? RfqId { get; set; }

        public Guid? QuoteId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContractNumber { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // 'draft', 'pending', 'signed', 'expired', 'terminated'

        public DateTime? SignedAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ContractTemplate? ContractTemplate { get; set; }
        public virtual BuyerProfile? BuyerProfile { get; set; }
        public virtual SellerProfile? SellerProfile { get; set; }
        public virtual RFQ? Rfq { get; set; }
        public virtual Quote? Quote { get; set; }
    }
}
