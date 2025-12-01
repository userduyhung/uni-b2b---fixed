using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for Cart operations
    /// </summary>
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cartRepository">Cart repository</param>
        /// <param name="userRepository">User repository</param>
        /// <param name="productRepository">Product repository</param>
        public CartService(ICartRepository cartRepository, IUserRepository userRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Create a new cart for the user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Created cart</returns>
        public async Task<Cart> CreateCartAsync(string userId)
        {
            // Validate user exists
            var userIdGuid = Guid.Parse(userId);
            var user = await _userRepository.GetUserByIdAsync(userIdGuid);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

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

        /// <summary>
        /// Add an item to the cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cartId">Cart ID</param>
        /// <param name="productId">Product ID</param>
        /// <param name="quantity">Quantity to add</param>
        /// <param name="price">Unit price of the product</param>
        /// <returns>Added cart item</returns>
        public async Task<CartItem> AddItemToCartAsync(string userId, string cartId, string productId, int quantity, decimal price)
        {
            // Validate user exists
            var userIdGuid = Guid.Parse(userId);
            var user = await _userRepository.GetUserByIdAsync(userIdGuid);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Validate product exists
            var productIdGuid = Guid.Parse(productId);
            var product = await _productRepository.GetByIdAsync(productIdGuid);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            // Get or create cart
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                cart = new Cart
                {
                    Id = cartId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                cart = await _cartRepository.CreateCartAsync(cart);
            }
            else if (cart.UserId != userId)
            {
                throw new UnauthorizedAccessException("Cart does not belong to user");
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

        /// <summary>
        /// Get cart items
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cartId">Cart ID</param>
        /// <returns>Collection of cart items</returns>
        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId, string cartId)
        {
            // Validate user exists
            var userIdGuid = Guid.Parse(userId);
            var user = await _userRepository.GetUserByIdAsync(userIdGuid);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Validate cart exists and belongs to user
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null || cart.UserId != userId)
            {
                throw new UnauthorizedAccessException("Cart does not belong to user");
            }

            return await _cartRepository.GetCartItemsAsync(cartId);
        }

        /// <summary>
        /// Update a cart item
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cartId">Cart ID</param>
        /// <param name="productId">Product ID</param>
        /// <param name="quantity">New quantity</param>
        /// <returns>Updated cart item</returns>
        public async Task<CartItem> UpdateCartItemAsync(string userId, string cartId, string productId, int quantity)
        {
            // Validate user exists
            var userIdGuid = Guid.Parse(userId);
            var user = await _userRepository.GetUserByIdAsync(userIdGuid);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Validate cart exists and belongs to user
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null || cart.UserId != userId)
            {
                throw new UnauthorizedAccessException("Cart does not belong to user");
            }

            var cartItem = await GetCartItemInCart(cartId, productId);
            if (cartItem == null)
            {
                throw new ArgumentException("Cart item not found");
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

        /// <summary>
        /// Remove an item from the cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cartId">Cart ID</param>
        /// <param name="productId">Product ID</param>
        /// <returns>True if successfully removed, false otherwise</returns>
        public async Task<bool> RemoveItemFromCartAsync(string userId, string cartId, string productId)
        {
            // Validate user exists
            var userIdGuid = Guid.Parse(userId);
            var user = await _userRepository.GetUserByIdAsync(userIdGuid);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Validate cart exists and belongs to user
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

        /// <summary>
        /// Remove all items from the cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cartId">Cart ID</param>
        /// <returns>True if successfully cleared, false otherwise</returns>
        public async Task<bool> ClearCartAsync(string userId, string cartId)
        {
            // Validate user exists
            var userIdGuid = Guid.Parse(userId);
            var user = await _userRepository.GetUserByIdAsync(userIdGuid);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Validate cart exists and belongs to user
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null || cart.UserId != userId)
            {
                throw new UnauthorizedAccessException("Cart does not belong to user");
            }

            var cartItems = await _cartRepository.GetCartItemsAsync(cartId);
            foreach (var item in cartItems)
            {
                await _cartRepository.RemoveItemFromCartAsync(item.Id);
            }

            return true;
        }

        /// <summary>
        /// Get cart by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Cart entity or null if not found</returns>
        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }

        private async Task<CartItem> GetCartItemInCart(string cartId, string productId)
        {
            var cartItems = await _cartRepository.GetCartItemsAsync(cartId);
            return cartItems.FirstOrDefault(ci => ci.ProductId == productId);
        }
    }
}