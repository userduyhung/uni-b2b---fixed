using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class PublicProfileTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PublicProfileTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPublicSellerProfiles_ReturnsProfiles()
    {
        var response = await _client.GetAsync("/api/public/sellers");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPublicSellerProfiles_WithPagination_ReturnsProfiles()
    {
        var response = await _client.GetAsync("/api/public/sellers?page=1&pageSize=5");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPublicSellerProfiles_WithFilters_ReturnsProfiles()
    {
        var response = await _client.GetAsync("/api/public/sellers?industry=Technology&country=USA");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
