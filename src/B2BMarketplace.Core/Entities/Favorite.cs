using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a favorite relationship between a buyer and a seller
    /// </summary>
    public class Favorite
    {
        /// <summary>
        /// Unique identifier for the favorite relationship
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the buyer profile
        /// </summary>
        public Guid BuyerProfileId { get; set; }

        /// <summary>
        /// Foreign key to the seller profile
        /// </summary>
        public Guid SellerProfileId { get; set; }

        /// <summary>
        /// Timestamp when the favorite was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property to the buyer profile
        /// </summary>
        [ForeignKey("BuyerProfileId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public BuyerProfile BuyerProfile { get; set; } = null!;

        /// <summary>
        /// Navigation property to the seller profile
        /// </summary>
        [ForeignKey("SellerProfileId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public SellerProfile SellerProfile { get; set; } = null!;

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public Favorite()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
    }
}