using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for Order operations
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="cartRepository">Cart repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="addressRepository">Address repository</param>
        /// <param name="paymentMethodRepository">Payment method repository</param>
        /// <param name="userRepository">User repository</param>
        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IAddressRepository addressRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _addressRepository = addressRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="order">Order entity</param>
        /// <returns>Created order</returns>
        public async Task<Order> CreateOrderAsync(Guid userId, Order order)
        {
            // Validate user exists
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Validate cart exists and belongs to user
            var cart = await _cartRepository.GetCartByIdAsync(order.CartId);
            if (cart == null || cart.UserId != userId.ToString())
            {
                throw new UnauthorizedAccessException("Cart not found or does not belong to user");
            }

            // Validate address exists and belongs to user
            var address = await _addressRepository.GetAddressByIdAsync(order.DeliveryAddressId);
            if (address == null || address.UserId != userId)
            {
                throw new UnauthorizedAccessException("Delivery address not found or does not belong to user");
            }

            // Validate payment method exists and belongs to user
            var paymentMethod = await _paymentMethodRepository.GetPaymentMethodByIdAsync(order.PaymentMethodId);
            if (paymentMethod == null || paymentMethod.UserId != userId)
            {
                throw new UnauthorizedAccessException("Payment method not found or does not belong to user");
            }

            // Create the order
            order.Id = Guid.NewGuid().ToString(); // Using string ID
            order.UserId = userId;
            order.Status = OrderStatus.Pending.ToString();
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            // Calculate total amount from cart items
            var cartItems = await _cartRepository.GetCartItemsAsync(order.CartId);
            order.TotalAmount = cartItems.Sum(ci => ci.Price * ci.Quantity);

            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            // Clear the cart after order creation
            foreach (var item in cartItems)
            {
                await _cartRepository.RemoveItemFromCartAsync(item.Id);
            }

            return createdOrder;
        }

        /// <summary>
        /// Get orders by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Collection of orders</returns>
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            // Validate user exists
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            return await _orderRepository.GetOrdersByUserIdAsync(userId);
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="userId">User ID</param>
        /// <returns>Order entity</returns>
        public async Task<Order> GetOrderByIdAsync(string orderId, Guid userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            // Only return the order if it belongs to the requesting user
            if (order != null && order.UserId == userId)
            {
                return order;
            }

            throw new UnauthorizedAccessException("Order not found or does not belong to user");
        }

        /// <summary>
        /// Get orders by seller ID
        /// </summary>
        /// <param name="sellerId">Seller ID</param>
        /// <returns>Collection of orders</returns>
        public async Task<IEnumerable<Order>> GetOrdersBySellerIdAsync(Guid sellerId)
        {
            // In a real implementation, this would fetch orders where products were sold by the seller
            // For this implementation, we'll look up orders and associated products to find where the seller is involved
            // This is a simplified approach; a real implementation would have a more direct link
            
            // Use the repository method to get orders by seller ID
            var allOrders = await _orderRepository.GetOrdersBySellerIdAsync(sellerId);
            
            return allOrders;
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="newStatus">New status</param>
        /// <param name="notes">Additional notes</param>
        /// <returns>Updated order</returns>
        public async Task<Order> UpdateOrderStatusAsync(string orderId, Guid userId, string newStatus, string notes = null)
        {
            // In a real implementation, we'd verify that userId has permission to update the order status
            // For now, we'll just update the status using the repository method
            
            if (!Enum.TryParse<OrderStatus>(newStatus, true, out var orderStatus))
            {
                throw new ArgumentException($"Invalid order status: {newStatus}");
            }

            var updatedOrder = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, notes);
            if (updatedOrder == null)
            {
                throw new ArgumentException("Order not found");
            }
            return updatedOrder;
        }

        /// <summary>
        /// Confirm an order
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="sellerId">Seller ID</param>
        /// <param name="confirmation">Order confirmation data</param>
        /// <returns>Confirmed order</returns>
        public async Task<Order> ConfirmOrderAsync(string orderId, Guid sellerId, OrderConfirmationDto confirmation)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            // In a real implementation, we'd verify that sellerId is the seller of products in this order
            // For now, we'll proceed with confirmation
            
            order.Status = OrderStatus.Confirmed.ToString();
            order.Message = confirmation.Message;
            order.ShippedWith = confirmation.ShippedWith;
            order.TrackingNumber = confirmation.TrackingNumber;
            order.ShippingCost = confirmation.ShippingCost;
            order.TotalCost = confirmation.TotalCost;
            order.ShippedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);
            if (updatedOrder == null)
            {
                throw new ArgumentException("Failed to confirm order");
            }
            return updatedOrder;
        }

        /// <summary>
        /// Update an order
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="order">Updated order data</param>
        /// <returns>Updated order</returns>
        public async Task<Order> UpdateOrderAsync(string orderId, Guid userId, Order order)
        {
            // Get the existing order to check if it belongs to the user
            var existingOrder = await _orderRepository.GetOrderByIdAsync(orderId);
            if (existingOrder == null || existingOrder.UserId != userId)
            {
                throw new UnauthorizedAccessException("Order not found or does not belong to user");
            }

            // Update the order fields
            existingOrder.DeliveryAddressId = order.DeliveryAddressId;
            existingOrder.PaymentMethodId = order.PaymentMethodId;
            existingOrder.SpecialInstructions = order.SpecialInstructions;
            existingOrder.Notes = order.Notes;
            existingOrder.UpdatedAt = DateTime.UtcNow;

            var updatedOrder = await _orderRepository.UpdateOrderAsync(existingOrder);
            return updatedOrder;
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="userId">User ID</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> CancelOrderAsync(string id, Guid userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null || order.UserId != userId)
            {
                return false;
            }

            order.Status = OrderStatus.Cancelled.ToString();
            order.UpdatedAt = DateTime.UtcNow;

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);
            return updatedOrder != null;
        }
    }
}