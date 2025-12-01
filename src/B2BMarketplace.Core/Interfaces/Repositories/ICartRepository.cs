using B2BMarketplace.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> CreateCartAsync(Cart cart);
        Task<Cart> GetCartByIdAsync(string cartId);
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<CartItem> AddItemToCartAsync(CartItem cartItem);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(string cartId);
        Task<CartItem> GetCartItemByIdAsync(string cartItemId);
        Task<CartItem> UpdateCartItemAsync(CartItem cartItem);
        Task<bool> RemoveItemFromCartAsync(string cartItemId);
        Task<bool> SaveChangesAsync();
    }
}