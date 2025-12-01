using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class AuditLoggingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuditLoggingTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAuditLogs_Authenticated_ReturnsList()
    {
        var token = await GetAdminToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/admin/audit-logs?page=1&pageSize=50");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserAuditLogs_ValidUserId_ReturnsList()
    {
        var token = await GetAdminToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/admin/audit-logs/user/test-user-id");
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
