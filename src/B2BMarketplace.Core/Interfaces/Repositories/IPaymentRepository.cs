using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface for payment repository operations
    /// </summary>
    public interface IPaymentRepository
    {
        /// <summary>
        /// Adds a new payment
        /// </summary>
        /// <param name="payment">Payment to add</param>
        /// <returns>Added payment</returns>
        Task<Payment> AddPaymentAsync(Payment payment);

        /// <summary>
        /// Gets a payment by ID
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <returns>Payment if found, null otherwise</returns>
        Task<Payment?> GetPaymentByIdAsync(Guid paymentId);

        /// <summary>
        /// Gets a payment by ID (alias for GetPaymentByIdAsync)
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <returns>Payment if found, null otherwise</returns>
        Task<Payment?> GetByIdAsync(Guid paymentId);

        /// <summary>
        /// Updates a payment
        /// </summary>
        /// <param name="payment">Payment to update</param>
        /// <returns>Updated payment</returns>
        Task<Payment> UpdatePaymentAsync(Payment payment);

        /// <summary>
        /// Updates a payment (alias for UpdatePaymentAsync)
        /// </summary>
        /// <param name="payment">Payment to update</param>
        /// <returns>Updated payment</returns>
        Task<Payment> UpdateAsync(Payment payment);

        /// <summary>
        /// Gets payments for a specific seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of payments</returns>
        Task<IEnumerable<Payment>> GetPaymentsForSellerAsync(Guid sellerProfileId, int page, int pageSize);

        /// <summary>
        /// Gets pending payments
        /// </summary>
        /// <returns>List of pending payments</returns>
        Task<IEnumerable<Payment>> GetPendingPaymentsAsync();
    }

    /// <summary>
    /// Interface for premium subscription repository operations
    /// </summary>
    public interface IPremiumSubscriptionRepository
    {
        /// <summary>
        /// Adds a new premium subscription
        /// </summary>
        /// <param name="subscription">Subscription to add</param>
        /// <returns>Added subscription</returns>
        Task<PremiumSubscription> AddSubscriptionAsync(PremiumSubscription subscription);

        /// <summary>
        /// Adds a new premium subscription (alias for AddSubscriptionAsync)
        /// </summary>
        /// <param name="subscription">Subscription to add</param>
        /// <returns>Added subscription</returns>
        Task<PremiumSubscription> AddAsync(PremiumSubscription subscription);

        /// <summary>
        /// Gets a subscription by ID
        /// </summary>
        /// <param name="subscriptionId">Subscription ID</param>
        /// <returns>Subscription if found, null otherwise</returns>
        Task<PremiumSubscription?> GetSubscriptionByIdAsync(Guid subscriptionId);

        /// <summary>
        /// Gets the active subscription for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Active subscription if found, null otherwise</returns>
        Task<PremiumSubscription?> GetActiveSubscriptionForSellerAsync(Guid sellerProfileId);

        /// <summary>
        /// Updates a subscription
        /// </summary>
        /// <param name="subscription">Subscription to update</param>
        /// <returns>Updated subscription</returns>
        Task<PremiumSubscription> UpdateSubscriptionAsync(PremiumSubscription subscription);

        /// <summary>
        /// Gets subscriptions for a specific seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of subscriptions</returns>
        Task<IEnumerable<PremiumSubscription>> GetSubscriptionsForSellerAsync(Guid sellerProfileId, int page, int pageSize);

        /// <summary>
        /// Gets a subscription by payment ID
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <returns>Subscription if found, null otherwise</returns>
        Task<PremiumSubscription?> GetSubscriptionByPaymentIdAsync(Guid paymentId);
    }
}
