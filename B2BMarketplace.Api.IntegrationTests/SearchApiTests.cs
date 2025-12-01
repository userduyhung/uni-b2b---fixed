using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class SearchApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SearchApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task SearchProducts_ValidQuery_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search/products?query=electronics");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SearchProducts_WithCategory_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search/products?query=test&category=Electronics");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchSellers_ValidQuery_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search/sellers?query=company");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SearchRFQs_ValidQuery_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search/rfqs?query=electronics");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdvancedSearch_WithFilters_ReturnsResults()
    {
        var request = new
        {
            query = "electronics",
            category = "Electronics",
            minPrice = 10.0,
            maxPrice = 1000.0,
            location = "USA"
        };

        var response = await _client.PostAsJsonAsync("/api/search/advanced", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}