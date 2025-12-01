using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for PaymentInvoice entities
    /// </summary>
    public interface IPaymentInvoiceRepository
    {
        /// <summary>
        /// Creates a new payment invoice in the database
        /// </summary>
        /// <param name="paymentInvoice">Payment invoice entity to create</param>
        /// <returns>Created payment invoice entity</returns>
        Task<PaymentInvoice> CreateAsync(PaymentInvoice paymentInvoice);

        /// <summary>
        /// Updates an existing payment invoice in the database
        /// </summary>
        /// <param name="paymentInvoice">Payment invoice entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateAsync(PaymentInvoice paymentInvoice);

        /// <summary>
        /// Deletes a payment invoice by ID
        /// </summary>
        /// <param name="id">Payment invoice ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Gets a payment invoice by its ID
        /// </summary>
        /// <param name="id">Payment invoice ID to search for</param>
        /// <returns>Payment invoice entity if found, null otherwise</returns>
        Task<PaymentInvoice?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all payment invoices with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result containing payment invoices</returns>
        Task<PagedResult<PaymentInvoice>> GetAllAsync(int page, int pageSize);

        /// <summary>
        /// Gets invoice by payment ID
        /// </summary>
        /// <param name="paymentId">The payment ID</param>
        /// <returns>The payment invoice or null</returns>
        Task<PaymentInvoice?> GetByPaymentIdAsync(Guid paymentId);

        /// <summary>
        /// Gets invoices by seller profile ID
        /// </summary>
        /// <param name="sellerProfileId">The seller profile ID</param>
        /// <returns>Collection of payment invoices</returns>
        Task<IEnumerable<PaymentInvoice>> GetBySellerProfileIdAsync(Guid sellerProfileId);

        /// <summary>
        /// Gets invoices by buyer profile ID
        /// </summary>
        /// <param name="buyerProfileId">The buyer profile ID</param>
        /// <returns>Collection of payment invoices</returns>
        Task<IEnumerable<PaymentInvoice>> GetByBuyerProfileIdAsync(Guid buyerProfileId);

        /// <summary>
        /// Gets invoice by invoice number
        /// </summary>
        /// <param name="invoiceNumber">The invoice number</param>
        /// <returns>The payment invoice or null</returns>
        Task<PaymentInvoice?> GetByInvoiceNumberAsync(string invoiceNumber);
    }
}