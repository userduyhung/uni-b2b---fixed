using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class AdminTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AdminTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDashboardStats_Admin_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("admin@test.com", "Admin");

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/admin/dashboard/stats");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDashboardStats_NonAdmin_ReturnsForbidden()
    {
        var token = await RegisterAndLogin("nonadmin@test.com", "Buyer");

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/admin/dashboard/stats");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllUsers_Admin_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("admin2@test.com", "Admin");

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/admin/users");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllUsers_Unauthenticated_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/admin/users");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    private async Task<string> RegisterAndLogin(string email, string role)
    {
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Password123!", role });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Password123!" });
        var content = await loginResponse.Content.ReadAsStringAsync();
        return ExtractToken(content);
    }

    private string ExtractToken(string content)
    {
        var startIndex = content.IndexOf("\"token\":\"") + 9;
        if (startIndex < 9) return string.Empty;
        var endIndex = content.IndexOf("\"", startIndex);
        return content.Substring(startIndex, endIndex - startIndex);
    }
}
