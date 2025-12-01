using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class NotificationsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public NotificationsApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetNotifications_AuthenticatedUser_ReturnsNotifications()
    {
        var token = await SetupUserWithProfile("notifications@example.com");

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/notifications");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task MarkNotificationAsRead_ValidId_ReturnsSuccess()
    {
        var token = await SetupUserWithProfile("markread@example.com");

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PutAsync("/api/notifications/1/read", null);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetNotificationPreferences_AuthenticatedUser_ReturnsPreferences()
    {
        var token = await SetupUserWithProfile("preferences@example.com");

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/notifications/preferences");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateNotificationPreferences_ValidRequest_ReturnsSuccess()
    {
        var token = await SetupUserWithProfile("updateprefs@example.com");

        var request = new
        {
            emailNotifications = true,
            smsNotifications = false,
            pushNotifications = true
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PutAsJsonAsync("/api/notifications/preferences", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    private async Task<string> SetupUserWithProfile(string email)
    {
        var registerRequest = new
        {
            email = email,
            password = "Password123!",
            role = "Buyer"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = email,
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        var profileRequest = new
        {
            companyName = "Test Company",
            industry = "Technology",
            description = "A test company",
            website = "https://test.com",
            country = "USA",
            city = "New York"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", profileRequest);

        return token;
    }

    private static string ExtractTokenFromResponse(string response)
    {
        var tokenStart = response.IndexOf("\"token\":\"") + 9;
        var tokenEnd = response.IndexOf("\"", tokenStart);
        return response.Substring(tokenStart, tokenEnd - tokenStart);
    }
}