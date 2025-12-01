using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class NotificationsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public NotificationsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetNotifications_AuthenticatedUser_ReturnsNotifications()
    {
        // Setup user
        var registerRequest = new
        {
            email = "notifyuser@example.com",
            password = "Password123!",
            role = "Buyer"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "notifyuser@example.com",
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/notifications");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task MarkNotificationAsRead_ValidId_ReturnsSuccess()
    {
        // Setup user
        var registerRequest = new
        {
            email = "markreaduser@example.com",
            password = "Password123!",
            role = "Seller"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "markreaduser@example.com",
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsync("/api/notifications/1/read", null);

        // Assert
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUnreadCount_AuthenticatedUser_ReturnsCount()
    {
        // Setup user
        var registerRequest = new
        {
            email = "countuser@example.com",
            password = "Password123!",
            role = "Buyer"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "countuser@example.com",
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/notifications/unread-count");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    private string ExtractTokenFromResponse(string response)
    {
        var tokenStart = response.IndexOf("\"token\":\"") + 9;
        var tokenEnd = response.IndexOf("\"", tokenStart);
        return response.Substring(tokenStart, tokenEnd - tokenStart);
    }
}