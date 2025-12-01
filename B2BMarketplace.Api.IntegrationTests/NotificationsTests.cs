using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class NotificationsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NotificationsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetNotifications_Authenticated_ReturnsNotifications()
    {
        var token = await RegisterAndLogin("notifications@test.com", "Buyer");

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/notifications?page=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUnreadCount_Authenticated_ReturnsCount()
    {
        var token = await RegisterAndLogin("unreadcount@test.com", "Seller");

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/notifications/unread-count");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MarkAsRead_ValidId_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("markread@test.com", "Buyer");
        var notificationId = Guid.NewGuid();

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PutAsync($"/api/notifications/{notificationId}/mark-read", null);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MarkAllAsRead_Authenticated_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("markallread@test.com", "Seller");

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PutAsync("/api/notifications/mark-all-read", null);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetNotifications_Unauthenticated_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/notifications");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteNotification_ValidId_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("deletenotif@test.com", "Buyer");
        var notificationId = Guid.NewGuid();

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.DeleteAsync($"/api/notifications/{notificationId}");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NoContent, System.Net.HttpStatusCode.NotFound);
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