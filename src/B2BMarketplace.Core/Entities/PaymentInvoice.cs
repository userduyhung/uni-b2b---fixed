using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents an invoice for a payment transaction
    /// </summary>
    public class PaymentInvoice
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        public Guid SellerProfileId { get; set; }

        [Required]
        public Guid BuyerProfileId { get; set; }

        [Required]
        [MaxLength(100)]
        public string InvoiceNumber { get; set; } = string.Empty;

        public string? Description { get; set; }

        [MaxLength(500)]
        public string? InvoiceFilePath { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "draft"; // 'draft', 'issued', 'paid', 'overdue', 'cancelled'

        [Required]
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DueAt { get; set; }

        public DateTime? PaidAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Payment? Payment { get; set; }
        public virtual SellerProfile? SellerProfile { get; set; }
        public virtual BuyerProfile? BuyerProfile { get; set; }
    }
}
