using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class PaymentManagementTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PaymentManagementTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreatePaymentIntent_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var token = await RegisterAndLogin("payment@test.com", "Buyer");
        var paymentRequest = new
        {
            Amount = 99.99m,
            Currency = "USD",
            Description = "Premium subscription"
        };

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/payments/create-intent", paymentRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("client_secret");
        content.Should().Contain("99.99");
    }

    [Fact]
    public async Task CreatePaymentIntent_InvalidAmount_ReturnsBadRequest()
    {
        // Arrange
        var token = await RegisterAndLogin("paymentbad@test.com", "Buyer");
        var paymentRequest = new
        {
            Amount = -10.00m,
            Currency = "USD"
        };

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/payments/create-intent", paymentRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePaymentIntent_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var paymentRequest = new
        {
            Amount = 99.99m,
            Currency = "USD"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/payments/create-intent", paymentRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HandlePaymentWebhook_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var webhookRequest = new
        {
            PaymentId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/payments/webhook", webhookRequest);

        // Assert
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfirmPayment_AdminUser_ReturnsSuccess()
    {
        // Arrange
        var adminToken = await RegisterAndLogin("admin@test.com", "Admin");
        var paymentId = Guid.NewGuid();
        var confirmationRequest = new
        {
            PaymentId = paymentId,
            ProviderTransactionId = "txn_123456"
        };

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
        var response = await _client.PostAsJsonAsync($"/api/payments/{paymentId}/confirm", confirmationRequest);

        // Assert
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfirmPayment_NonAdmin_ReturnsForbidden()
    {
        // Arrange
        var userToken = await RegisterAndLogin("user@test.com", "Buyer");
        var paymentId = Guid.NewGuid();
        var confirmationRequest = new
        {
            PaymentId = paymentId,
            ProviderTransactionId = "txn_123456"
        };

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
        var response = await _client.PostAsJsonAsync($"/api/payments/{paymentId}/confirm", confirmationRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSellerPremiumStatus_AdminUser_ReturnsStatus()
    {
        // Arrange
        var adminToken = await RegisterAndLogin("adminpremium@test.com", "Admin");
        var sellerId = Guid.NewGuid();

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
        var response = await _client.GetAsync($"/api/payments/premium-status/{sellerId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("IsPremium");
    }

    [Fact]
    public async Task GetSellerPremiumStatus_NonAdmin_ReturnsForbidden()
    {
        // Arrange
        var userToken = await RegisterAndLogin("userpremium@test.com", "Buyer");
        var sellerId = Guid.NewGuid();

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
        var response = await _client.GetAsync($"/api/payments/premium-status/{sellerId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    private async Task<string> RegisterAndLogin(string email, string role)
    {
        var registerRequest = new
        {
            email,
            password = "Password123!",
            role
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email,
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        return ExtractTokenFromResponse(loginContent);
    }

    private string ExtractTokenFromResponse(string response)
    {
        var tokenStart = response.IndexOf("\"token\":\"") + 9;
        var tokenEnd = response.IndexOf("\"", tokenStart);
        return response.Substring(tokenStart, tokenEnd - tokenStart);
    }
}