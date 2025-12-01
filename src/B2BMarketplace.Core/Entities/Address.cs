using System;

namespace B2BMarketplace.Core.Entities
{
    public class Address
    {
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string RecipientName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public User User { get; set; }
    }
}