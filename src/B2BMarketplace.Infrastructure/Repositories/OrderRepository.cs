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
            _context.Orders.Add(order);
            
            // Add order items and status history
            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    item.Id = System.Guid.NewGuid().ToString();
                    item.OrderId = order.Id;
                    item.CreatedAt = System.DateTime.UtcNow;
                    _context.OrderItems.Add(item);
                }
            }
            
            // Add initial status history
            var initialStatus = new OrderStatusHistory
            {
                Id = System.Guid.NewGuid().ToString(),
                OrderId = order.Id,
                Status = order.Status,
                Notes = "Order created",
                ChangedAt = System.DateTime.UtcNow
            };
            _context.OrderStatusHistories.Add(initialStatus);
            
            await _context.SaveChangesAsync();
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
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.StatusHistory)
                .Include(o => o.User) // Include buyer information
                .Where(o => o.SellerId == sellerId)
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
    }
}