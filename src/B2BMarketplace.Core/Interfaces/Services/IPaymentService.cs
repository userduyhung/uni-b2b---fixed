using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for payment service operations
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Initiates a payment for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="initiatePaymentDto">Payment details</param>
        /// <returns>Payment confirmation</returns>
        Task<PaymentConfirmationDto> InitiatePaymentAsync(Guid sellerProfileId, InitiatePaymentDto initiatePaymentDto);

        /// <summary>
        /// Processes a payment confirmation from the payment provider
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <param name="isSuccessful">Whether the payment was successful</param>
        /// <param name="providerTransactionId">Payment provider transaction ID</param>
        /// <param name="errorMessage">Error message if payment failed</param>
        /// <returns>True if processed successfully, false otherwise</returns>
        Task<bool> ProcessPaymentConfirmationAsync(Guid paymentId, bool isSuccessful, string? providerTransactionId, string? errorMessage);

        /// <summary>
        /// Gets payments for a specific seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of payments</returns>
        Task<IEnumerable<PaymentDto>> GetPaymentsForSellerAsync(Guid sellerProfileId, int page, int pageSize);

        /// <summary>
        /// Processes a refund for a payment
        /// </summary>
        /// <param name="refundPaymentDto">Refund details</param>
        /// <param name="adminUserId">ID of the admin processing the refund</param>
        /// <returns>True if refunded successfully, false otherwise</returns>
        Task<bool> ProcessRefundAsync(RefundPaymentDto refundPaymentDto, Guid adminUserId);

        /// <summary>
        /// Activates premium features for a seller based on successful payment
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="paymentId">Payment ID</param>
        /// <returns>True if activated successfully, false otherwise</returns>
        Task<bool> ActivatePremiumFeaturesAsync(Guid sellerProfileId, Guid paymentId);
    }
}