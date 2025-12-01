using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using B2BMarketplace.Core.Enums;

namespace B2BMarketplace.Core.DTOs
{
    public class RFQDto
    {
        public Guid Id { get; set; }
        public Guid BuyerProfileId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RFQStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }

    public class CreateRFQDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime? DeliveryDate { get; set; }

        public List<CreateRFQItemDto> Items { get; set; } = new();

        public List<Guid>? RecipientIds { get; set; }
    }

    public class UpdateRFQStatusDto
    {
        public RFQStatus Status { get; set; }
    }

    public class RFQItemDto
    {
        public Guid Id { get; set; }
        public Guid RFQId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    public class CreateRFQItemDto
    {
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = string.Empty;
    }

    public class RFQRecipientDto
    {
        public Guid Id { get; set; }
        public Guid RFQId { get; set; }
        public Guid SellerProfileId { get; set; }
    }

    public class QuoteDto
    {
        public Guid Id { get; set; }
        public Guid RFQId { get; set; }
        public Guid SellerProfileId { get; set; }
        public decimal Price { get; set; }
        public string DeliveryTime { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ValidUntil { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Conditions { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateQuoteDto
    {
        public Guid RFQId { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }
        
        public decimal TotalPrice { get; set; }

        public DateTime? DeliveryDate { get; set; }
        
        public string DeliveryTime { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public DateTime ValidUntil { get; set; }

        public string Notes { get; set; } = string.Empty;

        public string Conditions { get; set; } = string.Empty;
    }
}