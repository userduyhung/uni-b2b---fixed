using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class RFQApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RFQApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateRFQ_ValidRequest_ReturnsSuccess()
    {
        var token = await SetupBuyerWithProfile("rfqbuyer@example.com");

        var request = new
        {
            title = "Need Electronics Components",
            description = "Looking for high-quality electronic components",
            category = "Electronics",
            budget = 10000.00,
            deadline = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd")
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/rfq", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Need Electronics Components");
    }

    [Fact]
    public async Task GetRFQs_ReturnsRFQList()
    {
        var response = await _client.GetAsync("/api/rfq");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetRFQ_ValidId_ReturnsRFQ()
    {
        var token = await SetupBuyerWithProfile("getrfq@example.com");

        var createRequest = new
        {
            title = "Test RFQ for Retrieval",
            description = "Test description",
            category = "Technology",
            budget = 5000.00,
            deadline = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd")
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var createResponse = await _client.PostAsJsonAsync("/api/rfq", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var rfqId = ExtractIdFromResponse(createContent);

        var response = await _client.GetAsync($"/api/rfq/{rfqId}");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Test RFQ for Retrieval");
    }

    [Fact]
    public async Task SubmitQuote_ValidRequest_ReturnsSuccess()
    {
        var buyerToken = await SetupBuyerWithProfile("quotebuyer@example.com");
        var sellerToken = await SetupSellerWithProfile("quoteseller@example.com");

        // Create RFQ
        var rfqRequest = new
        {
            title = "RFQ for Quote Test",
            description = "Need quotes for this",
            category = "Manufacturing",
            budget = 8000.00,
            deadline = DateTime.UtcNow.AddDays(20).ToString("yyyy-MM-dd")
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", buyerToken);
        var rfqResponse = await _client.PostAsJsonAsync("/api/rfq", rfqRequest);
        var rfqContent = await rfqResponse.Content.ReadAsStringAsync();
        var rfqId = ExtractIdFromResponse(rfqContent);

        // Submit quote
        var quoteRequest = new
        {
            price = 7500.00,
            deliveryTime = "2 weeks",
            notes = "High quality components included"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", sellerToken);
        var response = await _client.PostAsJsonAsync($"/api/rfq/{rfqId}/quotes", quoteRequest);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    private async Task<string> SetupBuyerWithProfile(string email)
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
            companyName = "Buyer Company",
            industry = "Technology",
            description = "A buyer company",
            website = "https://buyer.com",
            country = "USA",
            city = "New York"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", profileRequest);

        return token;
    }

    private async Task<string> SetupSellerWithProfile(string email)
    {
        var registerRequest = new
        {
            email = email,
            password = "Password123!",
            role = "Seller"
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
            companyName = "Seller Company",
            industry = "Manufacturing",
            description = "A seller company",
            website = "https://seller.com",
            country = "USA",
            city = "Chicago"
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

    private static string ExtractIdFromResponse(string response)
    {
        var idStart = response.IndexOf("\"Id\":") + 5;
        var idEnd = response.IndexOf(",", idStart);
        return response.Substring(idStart, idEnd - idStart).Trim('"');
    }
}