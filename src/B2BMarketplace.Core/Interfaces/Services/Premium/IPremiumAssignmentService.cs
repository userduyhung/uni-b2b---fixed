using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services.Premium
{
    /// <summary>
    /// Service interface for premium status assignment
    /// </summary>
    public interface IPremiumAssignmentService
    {
        /// <summary>
        /// Assigns premium status to a seller
        /// </summary>
        /// <param name="sellerId">ID of the seller to assign premium status to</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <param name="expirationDate">Optional expiration date for the premium status</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> AssignPremiumStatusAsync(Guid sellerId, Guid adminId, DateTime? expirationDate = null);

        /// <summary>
        /// Removes premium status from a seller
        /// </summary>
        /// <param name="sellerId">ID of the seller to remove premium status from</param>
        /// <param name="adminId">ID of the admin performing the action</param>
        /// <param name="reason">Optional reason for removing premium status</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> RemovePremiumStatusAsync(Guid sellerId, Guid adminId, string? reason = null);

        /// <summary>
        /// Checks if a seller has premium status
        /// </summary>
        /// <param name="sellerId">ID of the seller to check</param>
        /// <returns>True if seller has premium status, false otherwise</returns>
        Task<bool> HasPremiumStatusAsync(Guid sellerId);

        /// <summary>
        /// Gets premium status information for a seller
        /// </summary>
        /// <param name="sellerId">ID of the seller</param>
        /// <returns>Premium status information or null if not found</returns>
        Task<PremiumStatusDto?> GetPremiumStatusAsync(Guid sellerId);

        /// <summary>
        /// Updates premium status expiration date
        /// </summary>
        /// <param name="sellerId">ID of the seller</param>
        /// <param name="expirationDate">New expiration date</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> UpdatePremiumExpirationAsync(Guid sellerId, DateTime? expirationDate);

        /// <summary>
        /// Gets all sellers with premium status
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Paginated list of sellers with premium status</returns>
        Task<PagedResultDto<PremiumStatusDto>> GetPremiumSellersAsync(int page = 1, int size = 10);

        /// <summary>
        /// Verifies premium payment and assigns status automatically
        /// </summary>
        /// <param name="paymentId">ID of the payment to verify</param>
        /// <param name="paymentDetails">Payment details for verification</param>
        /// <returns>True if payment is valid and status assigned, false otherwise</returns>
        Task<bool> VerifyAndAssignPremiumFromPaymentAsync(Guid paymentId, object paymentDetails);
    }
}