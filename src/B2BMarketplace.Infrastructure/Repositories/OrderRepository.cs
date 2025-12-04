using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            // ✅ DEBUG: Log OrderItems before saving
            Console.WriteLine($"🔍 CreateOrderAsync: OrderId={order.Id}");
            Console.WriteLine($"🔍 OrderItems count BEFORE save: {order.OrderItems?.Count ?? -1}");
            if (order.OrderItems != null && order.OrderItems.Any())
            {
                Console.WriteLine($"🔍 First item: ProductId={order.OrderItems.First().ProductId}, Name={order.OrderItems.First().ProductName}");
            }
            
            // 🔥 FIX: Extract OrderItems before adding Order to avoid EF tracking conflicts
            var orderItems = order.OrderItems?.ToList() ?? new List<OrderItem>();
            Console.WriteLine($"🔍 Extracted {orderItems.Count} OrderItems from Order");
            
            // Clear ALL navigation properties to avoid EF tracking issues
            order.User = null;
            order.DeliveryAddress = null;
            order.PaymentMethod = null;
            order.OrderItems = new List<OrderItem>(); // ⚠️ Clear to prevent auto-tracking
            order.StatusHistory = new List<OrderStatusHistory>(); // ⚠️ Clear to prevent auto-tracking
            
            // Add Order entity first (WITHOUT OrderItems)
            Console.WriteLine($"🔍 Adding Order entity to DbContext (without OrderItems)...");
            _context.Orders.Add(order);
            
            // Add OrderItems separately
            if (orderItems.Any())
            {
                Console.WriteLine($"🔍 Adding {orderItems.Count} OrderItems to DbContext separately...");
                foreach (var item in orderItems)
                {
                    // Ensure ID and FK are set
                    if (string.IsNullOrEmpty(item.Id))
                    {
                        item.Id = System.Guid.NewGuid().ToString();
                    }
                    item.OrderId = order.Id;
                    item.CreatedAt = System.DateTime.UtcNow;
                    item.Order = null; // Clear navigation
                    
                    Console.WriteLine($"  ✅ Adding OrderItem: Id={item.Id}, ProductId={item.ProductId}, Qty={item.Quantity}");
                    _context.OrderItems.Add(item);
                }
            }
            else
            {
                Console.WriteLine("⚠️ WARNING: No OrderItems to add!");
            }
            
            // Add initial status history
            var initialStatus = new OrderStatusHistory
            {
                Id = System.Guid.NewGuid().ToString(),
                OrderId = order.Id,
                Status = order.Status,
                Notes = "Order created",
                ChangedAt = System.DateTime.UtcNow,
                Order = null // Clear navigation
            };
            Console.WriteLine($"🔍 Adding OrderStatusHistory to DbContext...");
            _context.OrderStatusHistories.Add(initialStatus);
            
            Console.WriteLine($"🔍 About to SaveChangesAsync...");
            var changeCount = await _context.SaveChangesAsync();
            Console.WriteLine($"✅ SaveChangesAsync completed: {changeCount} changes saved");
            
            // ✅ Verify OrderItems were saved
            var savedOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == order.Id);
            Console.WriteLine($"🔍 AFTER save - OrderItems count in DB: {savedOrder?.OrderItems?.Count ?? -1}");
            
            return order;
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.StatusHistory)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PaymentMethod)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.StatusHistory)
                .Include(o => o.DeliveryAddress)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersBySellerIdAsync(Guid sellerId)
        {
            // FIX: sellerId parameter is now SellerProfile.Id
            // We need to find the SellerProfile first to get its UserId
            // Then query Orders where Order.SellerId == SellerProfile.UserId
            
            var sellerProfile = await _context.SellerProfiles.FirstOrDefaultAsync(s => s.Id == sellerId);
            if (sellerProfile == null)
            {
                return new List<Order>();
            }

            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.StatusHistory)
                .Include(o => o.DeliveryAddress) // Include delivery address
                .Include(o => o.User) // Include buyer information
                .ThenInclude(u => u.BuyerProfile) // Include buyer name from BuyerProfile
                .Where(o => o.SellerId == sellerProfile.UserId)  // Use SellerProfile.UserId instead of sellerId
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrderStatusAsync(string orderId, string newStatus, string notes = null)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) 
            {
                throw new ArgumentException("Order not found");
            }

            // Update the order status
            var oldStatus = order.Status;
            order.Status = newStatus;
            order.UpdatedAt = System.DateTime.UtcNow;

            // Add status history entry
            var statusHistory = new OrderStatusHistory
            {
                Id = System.Guid.NewGuid().ToString(),
                OrderId = orderId,
                Status = newStatus,
                Notes = notes ?? $"Status changed from {oldStatus} to {newStatus}",
                ChangedAt = System.DateTime.UtcNow
            };
            _context.OrderStatusHistories.Add(statusHistory);

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // Admin methods
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.StatusHistory)
                .Include(o => o.User) // Buyer
                    .ThenInclude(u => u.BuyerProfile)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PaymentMethod)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithItemsPaginatedAsync(int page, int pageSize)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.StatusHistory)
                .Include(o => o.User) // Buyer
                    .ThenInclude(u => u.BuyerProfile)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PaymentMethod)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }
    }
}