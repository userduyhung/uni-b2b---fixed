using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IAddressService _addressService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartService cartService,
            IAddressService addressService,
            IPaymentMethodService paymentMethodService,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _cartService = cartService;
            _addressService = addressService;
            _paymentMethodService = paymentMethodService;
            _productRepository = productRepository;
        }

        public async Task<Order> CreateOrderAsync(Guid userId, Order order)
        {
            Console.WriteLine($"🔵 OrderService.CreateOrderAsync CALLED");
            Console.WriteLine($"🔵 UserId: {userId}");
            Console.WriteLine($"🔵 CartId: {order.CartId}");
            
            // Validate that the cart, address, and payment method belong to the user
            var userIdString = userId.ToString();
            var cart = await _cartService.GetCartItemsAsync(userIdString, order.CartId);
            var cartItemsList = new List<CartItem>(cart);
            
            Console.WriteLine($"🔵 CartItems retrieved: {cartItemsList.Count} items");
            if (cartItemsList.Any())
            {
                foreach (var item in cartItemsList)
                {
                    Console.WriteLine($"  - CartItem: ProductId={item.ProductId}, Qty={item.Quantity}, Price={item.Price}");
                }
            }
            else
            {
                Console.WriteLine($"⚠️ WARNING: Cart is EMPTY! CartId={order.CartId}, UserId={userId}");
            }
            
            if (cartItemsList.Count == 0)
            {
                throw new ArgumentException("Invalid cart or cart does not belong to user");
            }

            var address = await _addressService.GetAddressByIdAsync(order.DeliveryAddressId, userId);
            if (address == null)
            {
                throw new ArgumentException("Invalid address or address does not belong to user");
            }

            var paymentMethod = await _paymentMethodService.GetPaymentMethodByIdAsync(order.PaymentMethodId, userId);
            if (paymentMethod == null)
            {
                throw new ArgumentException("Invalid payment method or payment method does not belong to user");
            }

            // Set the user ID to the one provided, not from the input
            order.UserId = userId;
            order.SellerId = Guid.Empty; // This will be set from first product in cart
            order.Id = Guid.NewGuid().ToString();
            order.Status = "Pending"; // Default status
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            
            // Set required fields with defaults if not provided
            order.Currency = order.Currency ?? "VND";
            order.Notes = order.Notes ?? string.Empty;
            order.Message = order.Message ?? string.Empty;
            order.TrackingNumber = order.TrackingNumber ?? string.Empty;
            order.ShippedWith = order.ShippedWith ?? string.Empty;

            // Calculate total amount and populate order items from cart items
            order.TotalAmount = 0;
            order.OrderItems = new List<OrderItem>();
            
            Console.WriteLine($"🔵 Creating OrderItems from {cartItemsList.Count} CartItems...");
            
            foreach (var cartItem in cartItemsList)
            {
                // Fetch product details to get the product name
                var product = await _productRepository.GetByIdAsync(Guid.Parse(cartItem.ProductId));
                
                Console.WriteLine($"  🔵 Product lookup: ProductId={cartItem.ProductId}, Found={product != null}, Name={product?.Name ?? "NULL"}");
                Console.WriteLine($"  🔵 Product.ImagePath: '{product?.ImagePath ?? "NULL"}'");
                
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = order.Id, // Set the order ID for the order item
                    ProductId = cartItem.ProductId,
                    ProductName = product?.Name ?? "Unknown Product", // REQUIRED field
                    ProductImage = product?.ImagePath ?? "", // Save product image
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Price,
                    TotalPrice = cartItem.Price * cartItem.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                
                Console.WriteLine($"  ✅ OrderItem created: Id={orderItem.Id}, ProductName={orderItem.ProductName}, ProductImage='{orderItem.ProductImage}', Qty={orderItem.Quantity}");
                
                order.OrderItems.Add(orderItem);
                order.TotalAmount += orderItem.TotalPrice;
                
                // Set SellerId from the first product (all products in cart should be from same seller)
                if (order.SellerId == Guid.Empty && product != null)
                {
                    order.SellerId = product.SellerProfileId;
                }
            }
            
            Console.WriteLine($"🔵 OrderItems creation completed: Total {order.OrderItems.Count} items, TotalAmount={order.TotalAmount}");
            Console.WriteLine($"🔵 Calling OrderRepository.CreateOrderAsync...");

            return await _orderRepository.CreateOrderAsync(order);
        }

        public async Task<Order> GetOrderByIdAsync(string orderId, Guid userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            
            // Verify that the order belongs to the user (either as buyer or seller)
            if (order == null || (order.UserId != userId && order.SellerId != userId))
            {
                return null;
            }

            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _orderRepository.GetOrdersByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Order>> GetOrdersBySellerIdAsync(Guid sellerId)
        {
            return await _orderRepository.GetOrdersBySellerIdAsync(sellerId);
        }

        public async Task<Order> UpdateOrderAsync(string orderId, Guid userId, Order order)
        {
            var existingOrder = await _orderRepository.GetOrderByIdAsync(orderId);
            
            // Verify that the order belongs to the user
            if (existingOrder == null || existingOrder.UserId != userId)
            {
                return null;
            }

            // Only allow updating certain fields after creation
            existingOrder.SpecialInstructions = order.SpecialInstructions;
            existingOrder.UpdatedAt = DateTime.UtcNow;

            return await _orderRepository.UpdateOrderAsync(existingOrder);
        }

        public async Task<Order> UpdateOrderStatusAsync(string orderId, Guid userId, string newStatus, string notes = null)
        {
            var existingOrder = await _orderRepository.GetOrderByIdAsync(orderId);
            
            // Verify that the order belongs to the user (either buyer or seller)
            if (existingOrder == null || (existingOrder.UserId != userId && existingOrder.SellerId != userId))
            {
                return null;
            }

            return await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, notes);
        }

        public async Task<Order> ConfirmOrderAsync(string orderId, Guid sellerId, OrderConfirmationDto confirmation)
        {
            var existingOrder = await _orderRepository.GetOrderByIdAsync(orderId);
            
            // Verify that the order belongs to the seller
            if (existingOrder == null || existingOrder.SellerId != sellerId)
            {
                return null;
            }

            // Update the order with confirmation details
            existingOrder.ShippedWith = confirmation.ShippedWith;
            existingOrder.TrackingNumber = confirmation.TrackingNumber;
            existingOrder.ShippingCost = confirmation.ShippingCost;
            existingOrder.UpdatedAt = DateTime.UtcNow;

            // Update status to confirmed
            var updatedOrder = await _orderRepository.UpdateOrderStatusAsync(orderId, "Confirmed", confirmation.Message);

            return updatedOrder;
        }

        // Admin methods
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<Order> GetOrderByIdAdminAsync(string orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<Order> UpdateOrderStatusAdminAsync(string orderId, string newStatus, string notes = null)
        {
            var adminNotes = notes ?? $"Status updated by admin to {newStatus}";
            return await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, adminNotes);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithItemsAsync(int page, int pageSize)
        {
            return await _orderRepository.GetAllOrdersWithItemsPaginatedAsync(page, pageSize);
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _orderRepository.GetTotalOrdersCountAsync();
        }
    }
}