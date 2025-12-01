using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BMarketplace.Core.Entities
{
    /// <summary>
    /// Represents a buyer's profile information
    /// </summary>
    public class BuyerProfile
    {
        /// <summary>
        /// Unique identifier for the buyer profile
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the User entity
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Buyer's full name
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Buyer's company name
        /// </summary>
        [StringLength(255)]
        public string? CompanyName { get; set; }

        /// <summary>
        /// Buyer's country
        /// </summary>
        [StringLength(100)]
        public string? Country { get; set; }

        /// <summary>
        /// Buyer's phone number
        /// </summary>
        [StringLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// Timestamp when profile was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when profile was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Navigation property to the User entity
        /// </summary>
        [ForeignKey("UserId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public User User { get; set; } = null!;

        /// <summary>
        /// Navigation property to RFQs created by this buyer
        /// </summary>
        public List<RFQ> CreatedRFQs { get; set; } = new();

        /// <summary>
        /// Navigation property to favorites created by this buyer
        /// </summary>
        public List<Favorite> Favorites { get; set; } = new();

        /// <summary>
        /// Navigation property to reviews given by this buyer
        /// </summary>
        public List<Review> ReviewsGiven { get; set; } = new();

        /// <summary>
        /// Constructor to initialize default values
        /// </summary>
        public BuyerProfile()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}