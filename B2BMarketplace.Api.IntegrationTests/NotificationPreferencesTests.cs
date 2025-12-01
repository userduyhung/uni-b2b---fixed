using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class NotificationPreferencesTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NotificationPreferencesTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UpdateNotificationPreferences_ValidData_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("notiftest@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = new
        {
            emailNotificationsEnabled = true,
            rfqResponseNotifications = true,
            rfqReceivedNotifications = true,
            quoteUpdatedNotifications = true,
            certificationStatusNotifications = false,
            accountStatusNotifications = true
        };
        var response = await _client.PutAsJsonAsync("/api/notifications/preferences", request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    private async Task<string> RegisterAndLogin(string email)
    {
        await _client.PostAsJsonAsync("/api/Auth/register", new { email, password = "Pass123!", role = "Buyer" });
        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new { email, password = "Pass123!" });
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
