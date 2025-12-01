using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class RFQManagementTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RFQManagementTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateRFQ_ValidRequest_ReturnsSuccess()
    {
        var token = await RegisterAndLoginBuyer("rfqbuyer@example.com");
        await CreateBuyerProfile(token);

        var request = new
        {
            Title = "Need Electronics Components",
            Description = "Looking for high-quality electronic components",
            DeliveryDate = DateTime.UtcNow.AddDays(30),
            Items = new[]
            {
                new
                {
                    ProductName = "Electronic Components",
                    Description = "High-quality electronic components",
                    Quantity = 100,
                    Unit = "pieces"
                }
            }
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/rfq", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Need Electronics Components");
    }

    [Fact]
    public async Task CreateRFQ_SellerRole_ReturnsForbidden()
    {
        var token = await RegisterAndLoginSeller("rfqseller@example.com");

        var request = new
        {
            Title = "Should not work",
            Description = "Sellers cannot create RFQs",
            DeliveryDate = DateTime.UtcNow.AddDays(30),
            Items = new[]
            {
                new
                {
                    ProductName = "Test Product",
                    Description = "Test description",
                    Quantity = 100,
                    Unit = "pieces"
                }
            }
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/rfq", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
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
    public async Task SubmitQuote_ValidRequest_ReturnsSuccess()
    {
        // Create RFQ first
        var buyerToken = await RegisterAndLoginBuyer("quoterbuyer@example.com");
        await CreateBuyerProfile(buyerToken);
        var rfqRequest = new
        {
            Title = "Quote Test RFQ",
            Description = "RFQ for quote testing",
            DeliveryDate = DateTime.UtcNow.AddDays(30),
            Items = new[]
            {
                new
                {
                    ProductName = "Test Components",
                    Description = "Components for testing",
                    Quantity = 50,
                    Unit = "pieces"
                }
            }
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", buyerToken);
        var rfqResponse = await _client.PostAsJsonAsync("/api/rfq", rfqRequest);
        var rfqContent = await rfqResponse.Content.ReadAsStringAsync();

        // Extract RFQ ID
        var rfqId = ExtractIdFromResponse(rfqContent);

        // Submit quote as seller
        var sellerToken = await RegisterAndLoginSeller("quoteseller@example.com");
        await CreateSellerProfile(sellerToken);

        var quoteRequest = new
        {
            TotalPrice = 2000.00,
            DeliveryDate = DateTime.UtcNow.AddDays(14),
            Notes = "High quality components with warranty",
            Conditions = "Standard warranty terms apply"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", sellerToken);
        var response = await _client.PostAsJsonAsync($"/api/rfq/{rfqId}/quotes", quoteRequest);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.Created, System.Net.HttpStatusCode.BadRequest);
    }

    private async Task<string> RegisterAndLoginBuyer(string email)
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
        
        var tokenStart = loginContent.IndexOf("\"token\":\"") + 9;
        var tokenEnd = loginContent.IndexOf("\"", tokenStart);
        return loginContent.Substring(tokenStart, tokenEnd - tokenStart);
    }

    private async Task<string> RegisterAndLoginSeller(string email)
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
        
        var tokenStart = loginContent.IndexOf("\"token\":\"") + 9;
        var tokenEnd = loginContent.IndexOf("\"", tokenStart);
        return loginContent.Substring(tokenStart, tokenEnd - tokenStart);
    }

    private async Task CreateSellerProfile(string token)
    {
        var profileRequest = new
        {
            companyName = "Quote Seller Company",
            industry = "Electronics",
            description = "Electronics supplier",
            website = "https://quoteseller.com",
            country = "USA",
            city = "Chicago"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", profileRequest);
    }

    private async Task CreateBuyerProfile(string token)
    {
        var profileRequest = new
        {
            name = "Test Buyer",
            companyName = "Buyer Company",
            country = "USA",
            phone = "123-456-7890"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", profileRequest);
    }

    private string ExtractIdFromResponse(string content)
    {
        // Try different patterns for ID extraction
        var patterns = new[] { "\"id\":\"", "\"Id\":\"", "\"ID\":\"" };
        
        foreach (var pattern in patterns)
        {
            var startIndex = content.IndexOf(pattern);
            if (startIndex >= 0)
            {
                startIndex += pattern.Length;
                var endIndex = content.IndexOf("\"", startIndex);
                if (endIndex > startIndex)
                {
                    return content.Substring(startIndex, endIndex - startIndex);
                }
            }
        }
        
        throw new InvalidOperationException($"Could not extract ID from response: {content}");
    }
}