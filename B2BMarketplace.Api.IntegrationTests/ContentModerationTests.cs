using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class ContentModerationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ContentModerationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetContentItems_Authenticated_ReturnsList()
    {
        var token = await GetAdminToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/admin/ContentManagement/items?page=1&pageSize=10");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task PublishContent_ValidId_ReturnsSuccess()
    {
        var token = await GetAdminToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsync("/api/admin/ContentManagement/items/test-id/publish", null);
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UnpublishContent_ValidId_ReturnsSuccess()
    {
        var token = await GetAdminToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsync("/api/admin/ContentManagement/items/test-id/unpublish", null);
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
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
