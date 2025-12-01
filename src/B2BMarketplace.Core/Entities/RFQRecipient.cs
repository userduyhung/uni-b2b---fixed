using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    public class RFQRecipient
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RFQId { get; set; }

        public Guid SellerProfileId { get; set; }

        // Navigation properties
        [ForeignKey("RFQId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual RFQ RFQ { get; set; } = null!;

        [ForeignKey("SellerProfileId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual SellerProfile SellerProfile { get; set; } = null!;
    }
}