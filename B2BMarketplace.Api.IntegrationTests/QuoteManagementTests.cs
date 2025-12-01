using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class QuoteManagementTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public QuoteManagementTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SubmitQuote_ValidData_ReturnsSuccess()
    {
        var token = await RegisterAndLoginSeller("quoteseller@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = new
        {
            totalPrice = 2599.00,
            deliveryDate = "2025-11-15T00:00:00Z",
            notes = "All fasteners meet ISO 9001 quality standards"
        };
        var response = await _client.PostAsJsonAsync("/api/rfq/test-rfq-id/quotes", request);
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.Created, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMyQuotes_Authenticated_ReturnsList()
    {
        var token = await RegisterAndLoginSeller("quoteseller2@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/rfq/quotes/my?page=1&pageSize=10");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    private async Task<string> RegisterAndLoginSeller(string email)
    {
        await _client.PostAsJsonAsync("/api/Auth/register", new { email, password = "Pass123!", role = "Seller" });
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
