namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// DTO for adding a seller to favorites
    /// </summary>
    public class AddFavoriteRequest
    {
        /// <summary>
        /// Seller's profile ID to add to favorites
        /// </summary>
        public Guid SellerProfileId { get; set; }
    }

    /// <summary>
    /// DTO for removing a seller from favorites
    /// </summary>
    public class RemoveFavoriteRequest
    {
        /// <summary>
        /// Seller's profile ID to remove from favorites
        /// </summary>
        public Guid SellerProfileId { get; set; }
    }

    /// <summary>
    /// DTO for checking if a seller is favorited
    /// </summary>
    public class CheckFavoriteRequest
    {
        /// <summary>
        /// Seller's profile ID to check
        /// </summary>
        public Guid SellerProfileId { get; set; }
    }
}