using B2BMarketplace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(string orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<IEnumerable<Order>> GetOrdersBySellerIdAsync(Guid sellerId);
        Task<Order> UpdateOrderAsync(Order order);
        Task<Order> UpdateOrderStatusAsync(string orderId, string newStatus, string notes = null);
        Task<bool> SaveChangesAsync();
        
        // Admin methods
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetAllOrdersWithItemsPaginatedAsync(int page, int pageSize);
        Task<int> GetTotalOrdersCountAsync();
    }
}