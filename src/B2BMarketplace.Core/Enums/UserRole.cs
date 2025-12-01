namespace B2BMarketplace.Core.Enums
{
    /// <summary>
    /// Represents the different user roles in the B2B Marketplace platform
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Buyer role - can create RFQs and view sellers
        /// </summary>
        Buyer,

        /// <summary>
        /// Seller role - can create product listings and respond to RFQs
        /// </summary>
        Seller,

        /// <summary>
        /// Admin role - can manage users and system settings
        /// </summary>
        Admin
    }
}