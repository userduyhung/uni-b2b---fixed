using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using B2BMarketplace.Core.Models;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for payment invoice data access operations
    /// </summary>
    public class PaymentInvoiceRepository : IPaymentInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for PaymentInvoiceRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public PaymentInvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new payment invoice in the database
        /// </summary>
        /// <param name="paymentInvoice">Payment invoice entity to create</param>
        /// <returns>Created payment invoice entity</returns>
        public async Task<PaymentInvoice> CreateAsync(PaymentInvoice paymentInvoice)
        {
            var entry = await _context.PaymentInvoices.AddAsync(paymentInvoice);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing payment invoice in the database
        /// </summary>
        /// <param name="paymentInvoice">Payment invoice entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(PaymentInvoice paymentInvoice)
        {
            try
            {
                _context.PaymentInvoices.Update(paymentInvoice);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a payment invoice by ID
        /// </summary>
        /// <param name="id">Payment invoice ID to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var paymentInvoice = await _context.PaymentInvoices.FindAsync(id);
            if (paymentInvoice == null)
                return false;

            _context.PaymentInvoices.Remove(paymentInvoice);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a payment invoice by its ID
        /// </summary>
        /// <param name="id">Payment invoice ID to search for</param>
        /// <returns>Payment invoice entity if found, null otherwise</returns>
        public async Task<PaymentInvoice?> GetByIdAsync(Guid id)
        {
            return await _context.PaymentInvoices.FindAsync(id);
        }

        /// <summary>
        /// Gets all payment invoices with pagination
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paged result containing payment invoices</returns>
        public async Task<PagedResult<PaymentInvoice>> GetAllAsync(int page, int pageSize)
        {
            // Ensure page and pageSize are valid
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit max page size

            // Build query
            var query = _context.PaymentInvoices.AsQueryable();

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(pi => pi.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<PaymentInvoice>(items, totalItems, page, pageSize);
        }

        /// <summary>
        /// Gets invoice by payment ID
        /// </summary>
        /// <param name="paymentId">The payment ID</param>
        /// <returns>The payment invoice or null</returns>
        public async Task<PaymentInvoice?> GetByPaymentIdAsync(Guid paymentId)
        {
            return await _context.PaymentInvoices
                .FirstOrDefaultAsync(pi => pi.PaymentId == paymentId);
        }

        /// <summary>
        /// Gets invoices by seller profile ID
        /// </summary>
        /// <param name="sellerProfileId">The seller profile ID</param>
        /// <returns>Collection of payment invoices</returns>
        public async Task<IEnumerable<PaymentInvoice>> GetBySellerProfileIdAsync(Guid sellerProfileId)
        {
            return await _context.PaymentInvoices
                .Where(pi => pi.SellerProfileId == sellerProfileId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets invoices by buyer profile ID
        /// </summary>
        /// <param name="buyerProfileId">The buyer profile ID</param>
        /// <returns>Collection of payment invoices</returns>
        public async Task<IEnumerable<PaymentInvoice>> GetByBuyerProfileIdAsync(Guid buyerProfileId)
        {
            return await _context.PaymentInvoices
                .Where(pi => pi.BuyerProfileId == buyerProfileId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets invoice by invoice number
        /// </summary>
        /// <param name="invoiceNumber">The invoice number</param>
        /// <returns>The payment invoice or null</returns>
        public async Task<PaymentInvoice?> GetByInvoiceNumberAsync(string invoiceNumber)
        {
            return await _context.PaymentInvoices
                .FirstOrDefaultAsync(pi => pi.InvoiceNumber == invoiceNumber);
        }
    }
}