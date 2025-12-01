using B2BMarketplace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Guid userId, Order order);
        Task<Order> GetOrderByIdAsync(string orderId, Guid userId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<IEnumerable<Order>> GetOrdersBySellerIdAsync(Guid sellerId);
        Task<Order> UpdateOrderAsync(string orderId, Guid userId, Order order);
        Task<Order> UpdateOrderStatusAsync(string orderId, Guid userId, string newStatus, string notes = null);
        Task<Order> ConfirmOrderAsync(string orderId, Guid sellerId, OrderConfirmationDto confirmation);
    }

    public class OrderConfirmationDto
    {
        public string Message { get; set; }
        public string ShippedWith { get; set; }
        public string TrackingNumber { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}