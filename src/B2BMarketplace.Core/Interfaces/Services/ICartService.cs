using B2BMarketplace.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface ICartService
    {
        Task<Cart> CreateCartAsync(string userId);
        Task<CartItem> AddItemToCartAsync(string userId, string cartId, string productId, int quantity, decimal price);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId, string cartId);
        Task<CartItem> UpdateCartItemAsync(string userId, string cartId, string productId, int quantity);
        Task<bool> RemoveItemFromCartAsync(string userId, string cartId, string productId);
    }
}