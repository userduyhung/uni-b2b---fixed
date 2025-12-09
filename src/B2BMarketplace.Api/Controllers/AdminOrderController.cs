using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace B2BMarketplace.Api.Controllers
{
    /// <summary>
    /// Admin controller for managing all orders in the system
    /// </summary>
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly ApplicationDbContext _context;

        public AdminOrderController(IOrderService orderService, ApplicationDbContext context, ITokenService tokenService) : base(tokenService)
        {
            _orderService = orderService;
            _context = context;
        }

        /// <summary>
        /// Get all orders in the system (Admin only)
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 50)</param>
        /// <returns>Paginated list of all orders</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                // Verify admin role
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }

                // Get all orders from the system
                var allOrders = await _orderService.GetAllOrdersAsync();
                
                if (allOrders == null)
                {
                    return Ok(new {
                        success = true,
                        data = new {
                            items = new System.Collections.Generic.List<Order>(),
                            total = 0,
                            page = page,
                            pageSize = pageSize,
                            totalPages = 0
                        }
                    });
                }
                
                // Convert to list for pagination
                var ordersList = new System.Collections.Generic.List<Order>(allOrders);
                var totalCount = ordersList.Count;
                var totalPages = (int)System.Math.Ceiling((double)totalCount / pageSize);
                
                // Apply pagination
                var pagedOrders = ordersList
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // ✨ Enrich orders with seller information from database
                var enrichedOrders = new System.Collections.Generic.List<object>();
                
                foreach (var order in pagedOrders)
                {
                    // Get seller information from Users table via SellerId
                    var seller = await _context.Users
                        .Include(u => u.SellerProfile)
                        .FirstOrDefaultAsync(u => u.Id == order.SellerId);
                    
                    var buyer = order.User; // Already included via navigation property
                    
                    enrichedOrders.Add(new {
                        id = order.Id,
                        status = order.Status,
                        cartId = order.CartId,
                        deliveryAddressId = order.DeliveryAddressId,
                        paymentMethodId = order.PaymentMethodId,
                        specialInstructions = order.SpecialInstructions,
                        totalAmount = order.TotalAmount,
                        createdAt = order.CreatedAt,
                        updatedAt = order.UpdatedAt,
                        shippedAt = order.ShippedAt,
                        deliveredAt = order.DeliveredAt,
                        shippedWith = order.ShippedWith,
                        trackingNumber = order.TrackingNumber,
                        notes = order.Notes,
                        
                        // ✨ Buyer information
                        buyerId = buyer?.Id.ToString(),
                        buyerEmail = buyer?.Email,
                        // Prefer the buyer's full Name, then CompanyName, then email as a fallback
                        buyerName = buyer?.BuyerProfile?.Name ?? buyer?.BuyerProfile?.CompanyName ?? buyer?.Email ?? "N/A",
                        
                        // ✨ Seller information
                        sellerId = seller?.Id.ToString(),
                        sellerEmail = seller?.Email,
                        sellerName = seller?.SellerProfile?.CompanyName ?? seller?.Email ?? "N/A",
                        
                        // Order items
                        items = order.OrderItems?.Select(item => new {
                            id = item.Id,
                            productId = item.ProductId,
                            productName = item.ProductName,
                            productImage = item.ProductImage,
                            quantity = item.Quantity,
                            unitPrice = item.UnitPrice,
                            totalPrice = item.TotalPrice
                        }).ToList()
                    });
                }

                return Ok(new {
                    success = true,
                    data = new {
                        items = enrichedOrders,
                        total = totalCount,
                        page = page,
                        pageSize = pageSize,
                        totalPages = totalPages
                    }
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Failed to retrieve orders: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Get order details by ID (Admin view with full information)
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            try
            {
                // Verify admin role
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }

                var order = await _orderService.GetOrderByIdAdminAsync(id);
                
                if (order == null)
                {
                    return NotFound(new { success = false, message = "Order not found" });
                }

                return Ok(new {
                    success = true,
                    data = order
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Failed to retrieve order: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Update order status (Admin override)
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="request">Status update request</param>
        /// <returns>Updated order</returns>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] AdminOrderStatusUpdateRequest request)
        {
            try
            {
                // Verify admin role
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }

                var updatedOrder = await _orderService.UpdateOrderStatusAdminAsync(id, request.Status, request.Notes);
                
                if (updatedOrder == null)
                {
                    return NotFound(new { success = false, message = "Order not found" });
                }

                return Ok(new {
                    success = true,
                    data = updatedOrder,
                    message = "Order status updated successfully"
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Failed to update order status: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Get order statistics for admin dashboard
        /// </summary>
        /// <returns>Order statistics</returns>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetOrderStatistics()
        {
            try
            {
                // Verify admin role
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }

                var allOrders = await _orderService.GetAllOrdersAsync();
                var ordersList = allOrders?.ToList() ?? new System.Collections.Generic.List<Order>();

                var statistics = new {
                    totalOrders = ordersList.Count,
                    pendingOrders = ordersList.Count(o => o.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
                    confirmedOrders = ordersList.Count(o => o.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase)),
                    shippedOrders = ordersList.Count(o => o.Status.Equals("Shipped", StringComparison.OrdinalIgnoreCase)),
                    deliveredOrders = ordersList.Count(o => o.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase)),
                    completedOrders = ordersList.Count(o => o.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase)),
                    cancelledOrders = ordersList.Count(o => o.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)),
                    totalRevenue = ordersList.Sum(o => o.TotalAmount)
                };

                return Ok(new {
                    success = true,
                    data = statistics
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Failed to retrieve statistics: {ex.Message}" 
                });
            }
        }
    }

    /// <summary>
    /// Request model for admin order status update
    /// </summary>
    public class AdminOrderStatusUpdateRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
