using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using B2BMarketplace.Api.Helpers;
using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Models;
using B2BMarketplace.Infrastructure.Data;

namespace B2BMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/admin/payments")]
    [Authorize(Roles = "Admin")]
    public class AdminPaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminPaymentsController> _logger;

        public AdminPaymentsController(ApplicationDbContext context, ILogger<AdminPaymentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Backfill payment descriptions from related order items for payments that have the default auto-created description or no description.
        /// </summary>
        [HttpPost("backfill-descriptions")]
        public async Task<ActionResult<ApiResponse<object>>> BackfillDescriptions()
        {
            try
            {
                // Find payments that are missing a friendly description or still have the auto-created message
                var candidates = await _context.Payments
                    .Where(p => string.IsNullOrEmpty(p.Description) || p.Description.StartsWith("Auto-created payment record for order"))
                    .ToListAsync();

                int updated = 0;
                foreach (var p in candidates)
                {
                    if (string.IsNullOrEmpty(p.OrderId)) continue;

                    var order = await _context.Orders
                        .Include(o => o.OrderItems)
                        .FirstOrDefaultAsync(o => o.Id == p.OrderId);

                    if (order == null || order.OrderItems == null || !order.OrderItems.Any()) continue;

                    var summary = string.Join(" & ", order.OrderItems.Select(oi => $"{oi.Quantity} {oi.ProductName}"));
                    if (!string.IsNullOrWhiteSpace(summary))
                    {
                        p.Description = summary;
                        p.UpdatedAt = DateTime.UtcNow;
                        _context.Payments.Update(p);
                        updated++;
                    }
                }

                if (updated > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return Ok(ApiResponse<object>.CreateSuccess(new { Updated = updated }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error backfilling payment descriptions");
                return StatusCode(500, ApiResponse<object>.CreateFailure("Internal server error", 500));
            }
        }

        /// <summary>
        /// Get paginated list of payments for admin
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<AdminPaymentDto>>>> GetPayments(int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;

                var query = from p in _context.Payments
                            join pi in _context.PaymentInvoices on p.Id equals pi.PaymentId into pis
                            from pi in pis.DefaultIfEmpty()
                            join bp in _context.BuyerProfiles on pi.BuyerProfileId equals bp.Id into bps
                            from bp in bps.DefaultIfEmpty()
                            join o in _context.Orders on p.OrderId equals o.Id into os
                            from o in os.DefaultIfEmpty()
                            join bp2 in _context.BuyerProfiles on o.UserId equals bp2.UserId into bp2s
                            from bp2 in bp2s.DefaultIfEmpty()
                            join sp in _context.SellerProfiles on p.SellerProfileId equals sp.Id into sps
                            from sp in sps.DefaultIfEmpty()
                            orderby p.CreatedAt descending
                            select new AdminPaymentDto
                            {
                                Id = p.Id,
                                Amount = p.Amount,
                                Currency = p.Currency,
                                PaymentProvider = p.PaymentProvider,
                                ProviderTransactionId = p.ProviderTransactionId,
                                Status = p.Status.ToString(),
                                PaymentMethod = p.PaymentMethod,
                                Description = p.Description,
                                CreatedAt = p.CreatedAt,
                                CompletedAt = p.CompletedAt,
                                SellerName = sp != null ? (sp.BusinessName ?? sp.CompanyName) : null,
                                BuyerName = bp != null ? bp.Name : (bp2 != null ? bp2.Name : null),
                                OrderId = p.OrderId
                            };

                var totalItems = await query.CountAsync();
                var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                var paged = new PagedResult<AdminPaymentDto>(items, totalItems, page, pageSize);
                return Ok(ApiResponse<PagedResult<AdminPaymentDto>>.CreateSuccess(paged));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching admin payments list");
                return StatusCode(500, ApiResponse<PagedResult<AdminPaymentDto>>.CreateFailure("Internal server error", 500));
            }
        }

        /// <summary>
        /// Get payment details by payment id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AdminPaymentDto>>> GetById(Guid id)
        {
            try
            {
                var result = await (from p in _context.Payments
                                    where p.Id == id
                                    join pi in _context.PaymentInvoices on p.Id equals pi.PaymentId into pis
                                    from pi in pis.DefaultIfEmpty()
                                    join bp in _context.BuyerProfiles on pi.BuyerProfileId equals bp.Id into bps
                                    from bp in bps.DefaultIfEmpty()
                                    join o in _context.Orders on p.OrderId equals o.Id into os
                                    from o in os.DefaultIfEmpty()
                                    join bp2 in _context.BuyerProfiles on o.UserId equals bp2.UserId into bp2s
                                    from bp2 in bp2s.DefaultIfEmpty()
                                    join sp in _context.SellerProfiles on p.SellerProfileId equals sp.Id into sps
                                    from sp in sps.DefaultIfEmpty()
                                    select new AdminPaymentDto
                                    {
                                        Id = p.Id,
                                        Amount = p.Amount,
                                        Currency = p.Currency,
                                        PaymentProvider = p.PaymentProvider,
                                        ProviderTransactionId = p.ProviderTransactionId,
                                        Status = p.Status.ToString(),
                                        PaymentMethod = p.PaymentMethod,
                                        Description = p.Description,
                                        CreatedAt = p.CreatedAt,
                                        CompletedAt = p.CompletedAt,
                                        SellerName = sp != null ? (sp.BusinessName ?? sp.CompanyName) : null,
                                        BuyerName = bp != null ? bp.Name : (bp2 != null ? bp2.Name : null),
                                        OrderId = p.OrderId
                                    }).FirstOrDefaultAsync();

                if (result == null) return NotFound(ApiResponse<AdminPaymentDto>.CreateNotFound("Payment not found"));

                return Ok(ApiResponse<AdminPaymentDto>.CreateSuccess(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching admin payment by id {PaymentId}", id);
                return StatusCode(500, ApiResponse<AdminPaymentDto>.CreateFailure("Internal server error", 500));
            }
        }

        /// <summary>
        /// Get payments associated with an order id (string order id)
        /// </summary>
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AdminPaymentDto>>>> GetByOrderId(string orderId)
        {
            try
            {
                // Attempt to find invoices that reference the provided order id via InvoiceNumber or Description
                var invoices = await _context.PaymentInvoices
                    .Where(pi => pi.InvoiceNumber == orderId || pi.Description == orderId)
                    .ToListAsync();

                if (!invoices.Any())
                {
                    // Try matching by PaymentInvoice.Id if provided as a GUID
                    if (Guid.TryParse(orderId, out var parsed))
                    {
                        var inv = await _context.PaymentInvoices.FindAsync(parsed);
                        if (inv != null) invoices = new List<PaymentInvoice> { inv };
                    }
                }

                var payments = new List<AdminPaymentDto>();
                foreach (var inv in invoices)
                {
                    var p = await _context.Payments.FindAsync(inv.PaymentId);
                    if (p == null) continue;
                    var buyer = await _context.BuyerProfiles.FindAsync(inv.BuyerProfileId);
                    var seller = await _context.SellerProfiles.FindAsync(inv.SellerProfileId);

                    payments.Add(new AdminPaymentDto
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        Currency = p.Currency,
                        PaymentProvider = p.PaymentProvider,
                        ProviderTransactionId = p.ProviderTransactionId,
                        Status = p.Status.ToString(),
                        PaymentMethod = p.PaymentMethod,
                        Description = p.Description,
                        CreatedAt = p.CreatedAt,
                        CompletedAt = p.CompletedAt,
                        SellerName = seller != null ? (seller.BusinessName ?? seller.CompanyName) : null,
                        BuyerName = buyer?.Name,
                        OrderId = orderId
                    });
                }

                return Ok(ApiResponse<IEnumerable<AdminPaymentDto>>.CreateSuccess(payments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payments by order id {OrderId}", orderId);
                return StatusCode(500, ApiResponse<IEnumerable<AdminPaymentDto>>.CreateFailure("Internal server error", 500));
            }
        }

        /// <summary>
        /// Get simple payment statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<ApiResponse<object>>> GetStatistics()
        {
            try
            {
                var totalPayments = await _context.Payments.CountAsync();
                var totalAmount = await _context.Payments.SumAsync(p => (decimal?)p.Amount) ?? 0m;
                var completed = await _context.Payments.CountAsync(p => p.Status == Core.Entities.PaymentStatus.Completed);
                var pending = await _context.Payments.CountAsync(p => p.Status == Core.Entities.PaymentStatus.Pending);
                var failed = await _context.Payments.CountAsync(p => p.Status == Core.Entities.PaymentStatus.Failed);

                var result = new
                {
                    TotalPayments = totalPayments,
                    TotalAmount = totalAmount,
                    CompletedCount = completed,
                    PendingCount = pending,
                    FailedCount = failed
                };

                return Ok(ApiResponse<object>.CreateSuccess(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing payment statistics");
                return StatusCode(500, ApiResponse<object>.CreateFailure("Internal server error", 500));
            }
        }
    }
}
