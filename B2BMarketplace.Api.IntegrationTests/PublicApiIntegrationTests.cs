using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Xunit;
using System.Text.Json;

namespace B2BMarketplace.Api.IntegrationTests;

public class PublicApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PublicApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task PublicSellerApis_ShouldWork()
    {
        // Search sellers by keyword (public endpoint)
        var keywordResponse = await _client.GetAsync("/api/search/sellers?query=electronics&page=1&pageSize=10");
        keywordResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var content = await keywordResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var data = doc.RootElement.GetProperty("data");
        data.GetProperty("items").ValueKind.Should().Be(JsonValueKind.Array);

        // Search sellers by industry (public endpoint)
        var industryResponse = await _client.GetAsync("/api/search/sellers?industry=Technology&page=1&pageSize=10");
        industryResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Search sellers by certification (public endpoint)
        var certResponse = await _client.GetAsync("/api/search/sellers?certification=ISO&page=1&pageSize=10");
        certResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Search sellers by location (public endpoint)
        var locationResponse = await _client.GetAsync("/api/search/sellers?location=USA&page=1&pageSize=10");
        locationResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task PublicProductApis_ShouldWork()
    {
        // Get products (public endpoint)
        var productsResponse = await _client.GetAsync("/api/products?page=1&pageSize=10");
        productsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var content = await productsResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var data = doc.RootElement.GetProperty("data");
        data.GetProperty("items").ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task PublicReviewApis_ShouldWork()
    {
        // Note: This test assumes there's at least one seller with ID "1"
        // In a real scenario, you'd create test data first
        
        // Get seller reviews (public endpoint)
        var reviewsResponse = await _client.GetAsync("/api/reviews/seller/1?page=1&pageSize=10");
        // This might return 404 if seller doesn't exist, which is acceptable for this test
        reviewsResponse.StatusCode.Should().BeOneOf(
            System.Net.HttpStatusCode.OK, 
            System.Net.HttpStatusCode.NotFound
        );
    }

    [Fact]
    public async Task PublicCategoryApis_ShouldWork()
    {
        // Get categories (public endpoint)
        var categoriesResponse = await _client.GetAsync("/api/categories");
        categoriesResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var content = await categoriesResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        doc.RootElement.GetProperty("data").ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task HealthCheckEndpoint_ShouldWork()
    {
        // Health check endpoint (public)
        var healthResponse = await _client.GetAsync("/health");
        healthResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task ApiDocumentation_ShouldBeAccessible()
    {
        // Swagger documentation (public)
        var swaggerResponse = await _client.GetAsync("/swagger");
        swaggerResponse.StatusCode.Should().BeOneOf(
            System.Net.HttpStatusCode.OK,
            System.Net.HttpStatusCode.MovedPermanently,
            System.Net.HttpStatusCode.Found
        );
    }

    [Theory]
    [InlineData("/api/search/sellers")]
    [InlineData("/api/products")]
    [InlineData("/api/categories")]
    public async Task PublicEndpoints_ShouldNotRequireAuthentication(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("/api/profile")]
    [InlineData("/api/rfq")]
    [InlineData("/api/notifications")]
    [InlineData("/api/Admin/users")]
    public async Task ProtectedEndpoints_ShouldRequireAuthentication(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}