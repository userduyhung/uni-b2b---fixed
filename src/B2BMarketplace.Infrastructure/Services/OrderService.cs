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
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly IPaymentRepository _paymentRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="cartRepository">Cart repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="addressRepository">Address repository</param>
        /// <param name="paymentMethodRepository">Payment method repository</param>
        /// <param name="userRepository">User repository</param>
        /// <param name="sellerProfileRepository">Seller profile repository</param>
        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IAddressRepository addressRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IUserRepository userRepository,
            ISellerProfileRepository sellerProfileRepository,
            IPaymentRepository paymentRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _addressRepository = addressRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _userRepository = userRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _paymentRepository = paymentRepository;
        }

        /// <summary>
        /// Create a new order (or multiple orders if cart has items from multiple sellers)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="order">Order entity with cart, address, payment info</param>
        /// <returns>Created order (first order if multiple sellers)</returns>
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

            // Get cart items first
            var cartItems = await _cartRepository.GetCartItemsAsync(order.CartId);
            if (cartItems == null || !cartItems.Any())
            {
                throw new ArgumentException("Cart is empty");
            }

            // ✨ NEW: Group cart items by seller
            Console.WriteLine($"🔵 CreateOrder: Grouping {cartItems.Count()} cart items by seller...");
            var itemsBySeller = new Dictionary<Guid, List<CartItem>>();
            
            foreach (var cartItem in cartItems)
            {
                var product = await _productRepository.GetByIdAsync(Guid.Parse(cartItem.ProductId));
                if (product == null)
                {
                    Console.WriteLine($"⚠️ Product {cartItem.ProductId} not found, skipping");
                    continue;
                }

                var sellerProfile = await _sellerProfileRepository.GetByIdAsync(product.SellerProfileId);
                if (sellerProfile == null)
                {
                    Console.WriteLine($"⚠️ Seller profile {product.SellerProfileId} not found for product {cartItem.ProductId}, skipping");
                    continue;
                }

                var sellerId = sellerProfile.UserId;
                if (!itemsBySeller.ContainsKey(sellerId))
                {
                    itemsBySeller[sellerId] = new List<CartItem>();
                }
                itemsBySeller[sellerId].Add(cartItem);
            }

            if (!itemsBySeller.Any())
            {
                throw new ArgumentException("No valid products found in cart");
            }

            Console.WriteLine($"✅ Found {itemsBySeller.Count} different sellers in cart");
            
            // ✨ Create one order per seller
            var createdOrders = new List<Order>();
            
            foreach (var sellerGroup in itemsBySeller)
            {
                var sellerId = sellerGroup.Key;
                var sellerItems = sellerGroup.Value;
                
                Console.WriteLine($"📦 Creating order for Seller {sellerId} with {sellerItems.Count} items");

                var newOrder = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    SellerId = sellerId,
                    CartId = order.CartId,
                    DeliveryAddressId = order.DeliveryAddressId,
                    PaymentMethodId = order.PaymentMethodId,
                    SpecialInstructions = order.SpecialInstructions ?? string.Empty,
                    Status = OrderStatus.Pending.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Currency = order.Currency ?? "VND",
                    Notes = order.Notes ?? string.Empty,
                    Message = order.Message ?? string.Empty,
                    TrackingNumber = order.TrackingNumber ?? string.Empty,
                    ShippedWith = order.ShippedWith ?? string.Empty,
                    ShippingCost = 0, // Could be calculated per seller
                    OrderItems = new List<OrderItem>()
                };

                // Calculate total for this seller's items
                decimal sellerTotal = 0;
                
                foreach (var cartItem in sellerItems)
                {
                    var product = await _productRepository.GetByIdAsync(Guid.Parse(cartItem.ProductId));
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = newOrder.Id,
                        ProductId = cartItem.ProductId,
                        ProductName = product?.Name ?? "Unknown Product",
                        ProductImage = product?.ImagePath ?? "",
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Price,
                        TotalPrice = cartItem.Price * cartItem.Quantity,
                        CreatedAt = DateTime.UtcNow
                    };
                    newOrder.OrderItems.Add(orderItem);
                    sellerTotal += orderItem.TotalPrice;
                }

                newOrder.TotalAmount = sellerTotal;
                newOrder.TotalCost = sellerTotal + newOrder.ShippingCost;
                
                Console.WriteLine($"💰 Order total for Seller {sellerId}: {sellerTotal:N0} VND");

                var createdOrder = await _orderRepository.CreateOrderAsync(newOrder);
                createdOrders.Add(createdOrder);
            }

            Console.WriteLine($"✅ Created {createdOrders.Count} orders successfully");

            // Create payment records for each created order so admin can observe transactions
            foreach (var createdOrder in createdOrders)
            {
                try
                {
                    await CreatePaymentForOrderAsync(createdOrder, paymentMethod, user);
                }
                catch (Exception ex)
                {
                    // Log and continue - payment record creation should not block order creation
                    Console.WriteLine($"⚠️ Failed to create payment record for Order {createdOrder.Id}: {ex.Message}");
                }
            }

            // Clear the cart after order creation
            foreach (var item in cartItems)
            {
                await _cartRepository.RemoveItemFromCartAsync(item.Id);
            }

            // Return the first order (for backward compatibility with existing API contract)
            // Note: In the future, consider returning List<Order> or a summary DTO
            return createdOrders.First();
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
        /// Create a payment record for an order (admin visibility / bookkeeping)
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="paymentMethod">Payment method used</param>
        /// <param name="buyer">Buyer user</param>
        /// <returns>Task</returns>
        private async Task CreatePaymentForOrderAsync(Order order, PaymentMethod paymentMethod, User buyer)
        {
            if (order == null) return;

                // Resolve the SellerProfile.Id (the Order.SellerId stores the SellerProfile.UserId)
                var sellerProfile = await _sellerProfileRepository.GetByUserIdAsync(order.SellerId);
                if (sellerProfile == null)
                {
                    Console.WriteLine($"⚠️ SellerProfile not found for UserId {order.SellerId}, skipping payment creation");
                    return;
                }

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    SellerProfileId = sellerProfile.Id,
                    OrderId = order.Id,
                    Amount = order.TotalAmount,
                    Currency = order.Currency ?? "VND",
                    PaymentProvider = string.Empty,
                    ProviderTransactionId = null,
                    Status = PaymentStatus.Pending,
                    PaymentMethod = paymentMethod?.Type ?? "unknown",
                    // Build a friendly description from order items, e.g. "2 Bia & 1 Nước lọc"
                    Description = (order.OrderItems != null && order.OrderItems.Any())
                        ? string.Join(" & ", order.OrderItems.Select(oi => $"{oi.Quantity} {oi.ProductName}"))
                        : $"Auto-created payment record for order {order.Id}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _paymentRepository.AddPaymentAsync(payment);
                // Reflect initial payment status on the order for quick admin view
                try
                {
                    order.PaymentStatus = payment.Status;
                    await _orderRepository.UpdateOrderAsync(order);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Failed to update Order.PaymentStatus for {order.Id}: {ex.Message}");
                }
        }

        /// <summary>
        /// Complete payment when order is delivered or admin marks it as paid
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Task</returns>
        public async Task CompletePaymentForOrderAsync(string orderId)
        {
            // Try to find a pending payment associated with this order
            var pendingPayments = await _paymentRepository.GetPendingPaymentsAsync();
            var payment = pendingPayments.FirstOrDefault(p => p.OrderId == orderId);

            if (payment == null)
            {
                Console.WriteLine($"⚠️ No pending payment found for order {orderId}");
                return;
            }

            payment.Status = PaymentStatus.Completed;
            payment.CompletedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdatePaymentAsync(payment);

            Console.WriteLine($"✅ Payment {payment.Id} for Order {orderId} marked as Completed");
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

            // Admin bypass: if userId == Guid.Empty, allow access to the order for admin endpoints
            if (userId == Guid.Empty)
            {
                return order;
            }

            // Return the order if it belongs to the requesting user (buyer) or the seller
            if (order != null && (order.UserId == userId || order.SellerId == userId))
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

        // Admin methods
        
        /// <summary>
        /// Get all orders in the system (Admin only)
        /// </summary>
        /// <returns>All orders</returns>
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        /// <summary>
        /// Get order by ID without user restriction (Admin view)
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Order details</returns>
        public async Task<Order> GetOrderByIdAdminAsync(string orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        /// <summary>
        /// Update order status without user restriction (Admin override)
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="newStatus">New status</param>
        /// <param name="notes">Admin notes</param>
        /// <returns>Updated order</returns>
        public async Task<Order> UpdateOrderStatusAdminAsync(string orderId, string newStatus, string notes = null)
        {
            if (!Enum.TryParse<OrderStatus>(newStatus, true, out var orderStatus))
            {
                throw new ArgumentException($"Invalid order status: {newStatus}");
            }

            var adminNotes = notes ?? $"Status updated by admin to {newStatus}";
            return await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, adminNotes);
        }

        /// <summary>
        /// Get all orders with items and pagination (Admin only)
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Items per page</param>
        /// <returns>Paginated orders with items</returns>
        public async Task<IEnumerable<Order>> GetAllOrdersWithItemsAsync(int page, int pageSize)
        {
            return await _orderRepository.GetAllOrdersWithItemsPaginatedAsync(page, pageSize);
        }

        /// <summary>
        /// Get total count of all orders (Admin only)
        /// </summary>
        /// <returns>Total order count</returns>
        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _orderRepository.GetTotalOrdersCountAsync();
        }
    }
}