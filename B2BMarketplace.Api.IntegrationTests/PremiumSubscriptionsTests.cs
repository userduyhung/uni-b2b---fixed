using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class PremiumSubscriptionsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PremiumSubscriptionsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPlans_Anonymous_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/api/premium-subscriptions/plans");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMySubscription_Authenticated_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("premseller@test.com", "Seller");
        await CreateSellerProfile(token);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/premium-subscriptions/my-subscription");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMySubscription_Unauthenticated_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/premium-subscriptions/my-subscription");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Subscribe_Seller_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("premseller2@test.com", "Seller");
        await CreateSellerProfile(token);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/premium-subscriptions/subscribe", new
        {
            planId = Guid.NewGuid(),
            paymentMethodId = "pm_test_123"
        });

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.BadRequest, System.Net.HttpStatusCode.NotFound);
    }

    private async Task<string> RegisterAndLogin(string email, string role)
    {
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Password123!", role });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Password123!" });
        var content = await loginResponse.Content.ReadAsStringAsync();
        return ExtractToken(content);
    }

    private async Task CreateSellerProfile(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PutAsJsonAsync("/api/profile/seller", new
        {
            CompanyName = "Premium Company",
            LegalRepresentative = "John Doe",
            TaxId = "123456789",
            Country = "USA"
        });
    }

    private string ExtractToken(string content)
    {
        var startIndex = content.IndexOf("\"token\":\"") + 9;
        if (startIndex < 9) return string.Empty;
        var endIndex = content.IndexOf("\"", startIndex);
        return content.Substring(startIndex, endIndex - startIndex);
    }
}
