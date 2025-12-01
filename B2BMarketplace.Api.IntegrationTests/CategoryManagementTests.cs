using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class CategoryManagementTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CategoryManagementTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllCategories_ReturnsSuccess()
    {
        var token = await GetAdminToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/admin/categories");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateCategory_ValidData_ReturnsCreated()
    {
        var token = await GetAdminToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = new
        {
            Name = "Test Category",
            Description = "Test Description",
            Slug = "test-category",
            DisplayOrder = 1,
            IsActive = true
        };
        var response = await _client.PostAsJsonAsync("/api/admin/categories", request);
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.Created);
    }

    private async Task<string> GetAdminToken()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new { email = "admin@b2bmarketplace.com", password = "AdminPass123!" });
        var content = await loginResponse.Content.ReadAsStringAsync();
        var jsonDocument = System.Text.Json.JsonDocument.Parse(content);
        if (jsonDocument.RootElement.TryGetProperty("data", out var dataElement))
        {
            if (dataElement.TryGetProperty("token", out var tokenElement))
            {
                return tokenElement.GetString() ?? string.Empty;
            }
        }
        return string.Empty;
    }
}
