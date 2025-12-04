using System;
using System.Collections.Generic;

namespace B2BMarketplace.Core.Entities
{
    public class Order
    {
        public string Id { get; set; }
        public Guid UserId { get; set; } // Buyer
        public Guid SellerId { get; set; }
        public string CartId { get; set; }
        public string DeliveryAddressId { get; set; }
        public string PaymentMethodId { get; set; }
        public string Status { get; set; } // e.g., 'Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled'
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public string SpecialInstructions { get; set; }
        public string TrackingNumber { get; set; }
        public string ShippedWith { get; set; }
        public decimal ShippingCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string Notes { get; set; }
        public string Message { get; set; }
        public decimal TotalCost { get; set; }
        
        // Navigation properties
        public User User { get; set; }
        public Address DeliveryAddress { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }

    public class OrderItem
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; } // Product image URL
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public Order Order { get; set; }
    }

    public class OrderStatusHistory
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime ChangedAt { get; set; }
        
        // Navigation properties
        public Order Order { get; set; }
    }
}