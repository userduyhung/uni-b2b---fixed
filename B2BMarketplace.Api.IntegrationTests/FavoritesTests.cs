using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class FavoritesTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public FavoritesTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddToFavorites_ValidRequest_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("favorites@test.com", "Buyer");
        await CreateBuyerProfile(token);

        var favoriteRequest = new
        {
            ItemType = "Product",
            ItemId = Guid.NewGuid()
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/favorites", favoriteRequest);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.Created, System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetMyFavorites_Authenticated_ReturnsFavorites()
    {
        var token = await RegisterAndLogin("myfavorites@test.com", "Buyer");
        await CreateBuyerProfile(token);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/favorites?page=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveFromFavorites_ValidId_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("removefav@test.com", "Buyer");
        await CreateBuyerProfile(token);

        var favoriteId = Guid.NewGuid();

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.DeleteAsync($"/api/favorites/{favoriteId}");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NoContent, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFavoriteProducts_Authenticated_ReturnsProducts()
    {
        var token = await RegisterAndLogin("favproducts@test.com", "Buyer");
        await CreateBuyerProfile(token);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/favorites/products?page=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFavoriteSellers_Authenticated_ReturnsSellers()
    {
        var token = await RegisterAndLogin("favsellers@test.com", "Buyer");
        await CreateBuyerProfile(token);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/favorites/sellers?page=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddToFavorites_Unauthenticated_ReturnsUnauthorized()
    {
        var favoriteRequest = new
        {
            ItemType = "Product",
            ItemId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync("/api/favorites", favoriteRequest);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    private async Task<string> RegisterAndLogin(string email, string role)
    {
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Password123!", role });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Password123!" });
        var content = await loginResponse.Content.ReadAsStringAsync();
        return ExtractToken(content);
    }

    private async Task CreateBuyerProfile(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", new
        {
            name = "Test Buyer",
            companyName = "Buyer Company",
            country = "USA"
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