using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class RFQIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RFQIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateRFQ_ValidRequest_ReturnsSuccess()
    {
        // Setup buyer
        var registerRequest = new
        {
            email = "rfqbuyer@example.com",
            password = "Password123!",
            role = "Buyer"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "rfqbuyer@example.com",
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Arrange
        var rfqRequest = new
        {
            Title = "Test RFQ",
            Description = "Looking for electronics components",
            Category = "Electronics",
            Budget = 5000.00,
            DeadlineDate = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd")
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rfq", rfqRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Test RFQ");
    }

    [Fact]
    public async Task GetRFQs_ReturnsRFQList()
    {
        // Act
        var response = await _client.GetAsync("/api/rfq");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RespondToRFQ_ValidRequest_ReturnsSuccess()
    {
        // Setup seller
        var registerRequest = new
        {
            email = "rfqseller@example.com",
            password = "Password123!",
            role = "Seller"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "rfqseller@example.com",
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create seller profile
        var profileRequest = new
        {
            companyName = "RFQ Test Company",
            industry = "Electronics",
            description = "Electronics supplier",
            website = "https://rfqtest.com",
            country = "USA",
            city = "Boston"
        };
        await _client.PostAsJsonAsync("/api/profile", profileRequest);

        // Arrange response
        var responseRequest = new
        {
            RFQId = 1,
            ProposedPrice = 4500.00,
            Message = "We can fulfill this requirement",
            DeliveryTimeframe = "2-3 weeks"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rfq/1/responses", responseRequest);

        // Assert
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.Created, System.Net.HttpStatusCode.OK);
    }

    private string ExtractTokenFromResponse(string response)
    {
        var tokenStart = response.IndexOf("\"token\":\"") + 9;
        var tokenEnd = response.IndexOf("\"", tokenStart);
        return response.Substring(tokenStart, tokenEnd - tokenStart);
    }
}