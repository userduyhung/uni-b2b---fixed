using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for payment operations
    /// </summary>
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for PaymentRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new payment
        /// </summary>
        /// <param name="payment">Payment to add</param>
        /// <returns>Added payment</returns>
        public async Task<Payment> AddPaymentAsync(Payment payment)
        {
            _ = _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        /// <summary>
        /// Gets a payment by ID
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <returns>Payment if found, null otherwise</returns>
        public async Task<Payment?> GetPaymentByIdAsync(Guid paymentId)
        {
            return await _context.Payments
                .Include(p => p.SellerProfile)
                .FirstOrDefaultAsync(p => p.Id == paymentId);
        }

        /// <summary>
        /// Gets a payment by ID (alias for GetPaymentByIdAsync)
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <returns>Payment if found, null otherwise</returns>
        public async Task<Payment?> GetByIdAsync(Guid paymentId)
        {
            return await GetPaymentByIdAsync(paymentId);
        }

        /// <summary>
        /// Updates a payment
        /// </summary>
        /// <param name="payment">Payment to update</param>
        /// <returns>Updated payment</returns>
        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            payment.UpdatedAt = DateTime.UtcNow;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        /// <summary>
        /// Updates a payment (alias for UpdatePaymentAsync)
        /// </summary>
        /// <param name="payment">Payment to update</param>
        /// <returns>Updated payment</returns>
        public async Task<Payment> UpdateAsync(Payment payment)
        {
            return await UpdatePaymentAsync(payment);
        }

        /// <summary>
        /// Gets payments for a specific seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of payments</returns>
        public async Task<IEnumerable<Payment>> GetPaymentsForSellerAsync(Guid sellerProfileId, int page, int pageSize)
        {
            return await _context.Payments
                .Include(p => p.SellerProfile)
                .Where(p => p.SellerProfileId == sellerProfileId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets pending payments
        /// </summary>
        /// <returns>List of pending payments</returns>
        public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.SellerProfile)
                .Where(p => p.Status == PaymentStatus.Pending)
                .ToListAsync();
        }
    }

    /// <summary>
    /// Repository for premium subscription operations
    /// </summary>
    public class PremiumSubscriptionRepository : IPremiumSubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for PremiumSubscriptionRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public PremiumSubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new premium subscription
        /// </summary>
        /// <param name="subscription">Subscription to add</param>
        /// <returns>Added subscription</returns>
        public async Task<PremiumSubscription> AddSubscriptionAsync(PremiumSubscription subscription)
        {
            _ = _context.PremiumSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        /// <summary>
        /// Adds a new premium subscription (alias for AddSubscriptionAsync)
        /// </summary>
        /// <param name="subscription">Subscription to add</param>
        /// <returns>Added subscription</returns>
        public async Task<PremiumSubscription> AddAsync(PremiumSubscription subscription)
        {
            return await AddSubscriptionAsync(subscription);
        }

        /// <summary>
        /// Gets a subscription by ID
        /// </summary>
        /// <param name="subscriptionId">Subscription ID</param>
        /// <returns>Subscription if found, null otherwise</returns>
        public async Task<PremiumSubscription?> GetSubscriptionByIdAsync(Guid subscriptionId)
        {
            return await _context.PremiumSubscriptions
                .Include(ps => ps.SellerProfile)
                .Include(ps => ps.Payment)
                .FirstOrDefaultAsync(ps => ps.Id == subscriptionId);
        }

        /// <summary>
        /// Gets the active subscription for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Active subscription if found, null otherwise</returns>
        public async Task<PremiumSubscription?> GetActiveSubscriptionForSellerAsync(Guid sellerProfileId)
        {
            return await _context.PremiumSubscriptions
                .Include(ps => ps.SellerProfile)
                .Include(ps => ps.Payment)
                .Where(ps => ps.SellerProfileId == sellerProfileId && ps.IsActive)
                .Where(ps => ps.EndDate == null || ps.EndDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Updates a subscription
        /// </summary>
        /// <param name="subscription">Subscription to update</param>
        /// <returns>Updated subscription</returns>
        public async Task<PremiumSubscription> UpdateSubscriptionAsync(PremiumSubscription subscription)
        {
            subscription.UpdatedAt = DateTime.UtcNow;
            _context.PremiumSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        /// <summary>
        /// Gets subscriptions for a specific seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of subscriptions</returns>
        public async Task<IEnumerable<PremiumSubscription>> GetSubscriptionsForSellerAsync(Guid sellerProfileId, int page, int pageSize)
        {
            return await _context.PremiumSubscriptions
                .Include(ps => ps.SellerProfile)
                .Include(ps => ps.Payment)
                .Where(ps => ps.SellerProfileId == sellerProfileId)
                .OrderByDescending(ps => ps.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a subscription by payment ID
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <returns>Subscription if found, null otherwise</returns>
        public async Task<PremiumSubscription?> GetSubscriptionByPaymentIdAsync(Guid paymentId)
        {
            return await _context.PremiumSubscriptions
                .Include(ps => ps.SellerProfile)
                .Include(ps => ps.Payment)
                .FirstOrDefaultAsync(ps => ps.PaymentId == paymentId);
        }
    }
}
