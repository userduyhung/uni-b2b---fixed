using System;

namespace B2BMarketplace.Core.Entities
{
    public class PaymentMethod
    {
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } // e.g., 'credit_card', 'debit_card', 'paypal', 'bank_account'
        public string CardNumber { get; set; } // Last 4 digits or masked
        public string ExpiryDate { get; set; } // Format: MM/YY
        public string CVV { get; set; } // Should be encrypted
        public string CardholderName { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public User User { get; set; }
    }
}