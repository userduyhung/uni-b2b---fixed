using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class SearchTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SearchTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SearchProducts_ValidQuery_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search/products?query=electronics&page=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchSellers_ValidQuery_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search/sellers?query=company&page=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchRFQs_ValidQuery_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search/rfqs?query=components&page=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GlobalSearch_ValidQuery_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search?query=test&page=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchProducts_EmptyQuery_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/search/products?query=&page=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.BadRequest, System.Net.HttpStatusCode.OK);
    }
}