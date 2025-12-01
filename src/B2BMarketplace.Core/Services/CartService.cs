using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<Cart> CreateCartAsync(string userId)
        {
            // Check if user already has an active cart
            var existingCart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (existingCart != null)
            {
                return existingCart;
            }

            var cart = new Cart
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _cartRepository.CreateCartAsync(cart);
        }

        public async Task<CartItem> AddItemToCartAsync(string userId, string cartId, string productId, int quantity, decimal price)
        {
            // Verify product exists
            // Convert string productId to Guid
            if (!Guid.TryParse(productId, out Guid productGuid))
            {
                throw new ArgumentException("Invalid product ID format");
            }
            
            var product = await _productRepository.GetByIdAsync(productGuid);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            // Check if item already exists in cart
            var existingCartItem = await GetCartItemInCart(cartId, productId);
            if (existingCartItem != null)
            {
                // Update quantity if item already exists
                existingCartItem.Quantity += quantity;
                existingCartItem.UpdatedAt = DateTime.UtcNow;
                return await _cartRepository.UpdateCartItemAsync(existingCartItem);
            }

            var cartItem = new CartItem
            {
                Id = Guid.NewGuid().ToString(),
                CartId = cartId,
                ProductId = productId,
                Quantity = quantity,
                Price = price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _cartRepository.AddItemToCartAsync(cartItem);
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId, string cartId)
        {
            // Verify that the cart belongs to the user
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null || cart.UserId != userId)
            {
                throw new UnauthorizedAccessException("Cart does not belong to user");
            }

            return await _cartRepository.GetCartItemsAsync(cartId);
        }

        public async Task<CartItem> UpdateCartItemAsync(string userId, string cartId, string productId, int quantity)
        {
            // Verify that the cart belongs to the user
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null || cart.UserId != userId)
            {
                throw new UnauthorizedAccessException("Cart does not belong to user");
            }

            var cartItem = await GetCartItemInCart(cartId, productId);
            if (cartItem == null)
            {
                throw new ArgumentException("Item not found in cart");
            }

            if (quantity <= 0)
            {
                // Remove item if quantity is 0 or less
                await _cartRepository.RemoveItemFromCartAsync(cartItem.Id);
                return null;
            }

            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            return await _cartRepository.UpdateCartItemAsync(cartItem);
        }

        public async Task<bool> RemoveItemFromCartAsync(string userId, string cartId, string productId)
        {
            // Verify that the cart belongs to the user
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null || cart.UserId != userId)
            {
                throw new UnauthorizedAccessException("Cart does not belong to user");
            }

            var cartItem = await GetCartItemInCart(cartId, productId);
            if (cartItem == null)
            {
                return false;
            }

            return await _cartRepository.RemoveItemFromCartAsync(cartItem.Id);
        }

        private async Task<CartItem> GetCartItemInCart(string cartId, string productId)
        {
            var cartItems = await _cartRepository.GetCartItemsAsync(cartId);
            return cartItems.FirstOrDefault(ci => ci.ProductId == productId);
        }
    }
}