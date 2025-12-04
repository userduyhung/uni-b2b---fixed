using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using B2BMarketplace.Infrastructure.Data;
using B2BMarketplace.Core.Entities;
using System;
using System.Linq;

namespace B2BMarketplace.Api.IntegrationTests;

public class PaymentDescriptionBackfillTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PaymentDescriptionBackfillTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async System.Threading.Tasks.Task BackfillEndpoint_UpdatesPaymentDescriptions()
    {
        // Arrange: insert an order with items and a payment that has the auto-created description
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // create buyer and seller users (required by FK and required fields)
        var sellerUserId = Guid.NewGuid();
        var buyerUserId = Guid.NewGuid();
        var sellerUser = new User { Id = sellerUserId, Email = $"seller_{sellerUserId}@test.local", PasswordHash = "testhash", Role = B2BMarketplace.Core.Enums.UserRole.Seller };
        var buyerUser = new User { Id = buyerUserId, Email = $"buyer_{buyerUserId}@test.local", PasswordHash = "testhash", Role = B2BMarketplace.Core.Enums.UserRole.Buyer };
        db.Users.Add(sellerUser);
        db.Users.Add(buyerUser);

        // create a seller profile (required fields populated)
        var seller = new SellerProfile
        {
            Id = Guid.NewGuid(),
            UserId = sellerUserId,
            CompanyName = "Test Company",
            LegalRepresentative = "John Doe",
            TaxId = "123456789",
            Country = "VN"
        };
        db.SellerProfiles.Add(seller);

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            UserId = buyerUserId,
            SellerId = sellerUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = new System.Collections.Generic.List<OrderItem>(),
            CartId = Guid.NewGuid().ToString(),
            DeliveryAddressId = string.Empty,
            PaymentMethodId = string.Empty,
            Status = "Pending",
            TotalAmount = 100m,
            Currency = "VND",
            SpecialInstructions = string.Empty,
            TrackingNumber = string.Empty,
            ShippedWith = string.Empty,
            ShippingCost = 0m,
            Notes = string.Empty,
            Message = string.Empty,
            TotalCost = 100m
        };

        var item1 = new OrderItem { Id = Guid.NewGuid().ToString(), OrderId = order.Id, ProductId = Guid.NewGuid().ToString(), ProductName = "Bia", Quantity = 2, UnitPrice = 30m, TotalPrice = 60m, CreatedAt = DateTime.UtcNow };
        var item2 = new OrderItem { Id = Guid.NewGuid().ToString(), OrderId = order.Id, ProductId = Guid.NewGuid().ToString(), ProductName = "Nước lọc", Quantity = 1, UnitPrice = 40m, TotalPrice = 40m, CreatedAt = DateTime.UtcNow };
        order.OrderItems.Add(item1);
        order.OrderItems.Add(item2);

        db.Orders.Add(order);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            SellerProfileId = seller.Id,
            OrderId = order.Id,
            Amount = 100m,
            Currency = "VND",
            Status = PaymentStatus.Pending,
            PaymentMethod = "cod",
            Description = $"Auto-created payment record for order {order.Id}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Payments.Add(payment);
        await db.SaveChangesAsync();

        // Register admin and login to get token
        var adminEmail = $"backfill_admin_{Guid.NewGuid().ToString().Substring(0,6)}@test.local";
        await _client.PostAsJsonAsync("/api/auth/register", new { email = adminEmail, password = "Password123!", role = "Admin" });
        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", new { email = adminEmail, password = "Password123!" });
        var loginContent = await loginResp.Content.ReadAsStringAsync();
        var token = ExtractToken(loginContent);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act: call backfill endpoint
        var resp = await _client.PostAsync("/api/admin/payments/backfill-descriptions", null);
        var respContent = await resp.Content.ReadAsStringAsync();
        // Provide a clearer failure message if the endpoint didn't succeed
        resp.IsSuccessStatusCode.Should().BeTrue($"Backfill endpoint returned {(int)resp.StatusCode} - {resp.StatusCode}: {respContent}");
        respContent.Should().Contain("Updated");

        // Assert: payment description updated
        var updated = await db.Payments.FindAsync(payment.Id);
        updated.Should().NotBeNull();
        updated!.Description.Should().Be("2 Bia & 1 Nước lọc", "Expected backfill to update payment description, but DB contains: '" + (updated.Description ?? "<null>") + "'");
    }

    private string ExtractToken(string content)
    {
        var startIndex = content.IndexOf("\"token\":\"") + 9;
        if (startIndex < 9) return string.Empty;
        var endIndex = content.IndexOf("\"", startIndex);
        return content.Substring(startIndex, endIndex - startIndex);
    }
}
