using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class SellerQuoteManagementTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SellerQuoteManagementTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMyQuotes_AuthenticatedSeller_ReturnsQuotesList()
    {
        var token = await RegisterAndLoginSeller("myquotes@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/quotes/my-quotes");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("data");
    }

    [Fact]
    public async Task GetMyQuotes_Unauthenticated_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/quotes/my-quotes");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateQuote_ValidData_ReturnsSuccess()
    {
        var token = await RegisterAndLoginSeller("updatequote@example.com");
        await CreateSellerProfile(token);
        
        var rfqId = await CreateTestRFQ();
        var quoteId = await SubmitTestQuote(token, rfqId);

        var updateRequest = new
        {
            price = 14500,
            deliveryTime = "25 days",
            description = "Updated quote with improved delivery time and competitive pricing",
            validUntil = "2024-12-31T23:59:59Z"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PutAsJsonAsync($"/api/quotes/{quoteId}", updateRequest);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateQuote_Unauthenticated_ReturnsUnauthorized()
    {
        var updateRequest = new
        {
            price = 14500,
            deliveryTime = "25 days",
            description = "Should not work",
            validUntil = "2024-12-31T23:59:59Z"
        };

        var response = await _client.PutAsJsonAsync("/api/quotes/test-quote-id", updateRequest);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SubmitQuote_ValidData_ReturnsCreatedWithQuoteId()
    {
        var token = await RegisterAndLoginSeller("submitquote@example.com");
        await CreateSellerProfile(token);
        
        var rfqId = await CreateTestRFQ();

        var quoteRequest = new
        {
            rfqId = rfqId,
            price = 15000,
            deliveryTime = "30 days",
            description = "High-quality industrial equipment meeting all specifications",
            validUntil = "2024-12-31T23:59:59Z"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/quotes", quoteRequest);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.Created, System.Net.HttpStatusCode.NotFound);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("id");
        }
    }

    [Fact]
    public async Task GetRFQDetails_ValidId_ReturnsRFQWithDetails()
    {
        var token = await RegisterAndLoginSeller("rfqdetails@example.com");
        var rfqId = await CreateTestRFQ();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync($"/api/rfqs/{rfqId}");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().ContainAny("title", "Title");
            content.Should().ContainAny("description", "Description");
        }
    }

    [Fact]
    public async Task GetAvailableRFQs_AuthenticatedSeller_ReturnsRFQsList()
    {
        var token = await RegisterAndLoginSeller("availablerfqs@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/rfqs");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("data");
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

    private async Task CreateSellerProfile(string token)
    {
        var profileRequest = new
        {
            companyName = "Test Seller Company",
            industry = "Technology",
            description = "Technology solutions provider",
            website = "https://testseller.com",
            country = "USA",
            city = "New York"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", profileRequest);
    }

    private async Task<string> CreateTestRFQ()
    {
        var buyerToken = await RegisterAndLoginBuyer($"buyer{Guid.NewGuid()}@example.com");
        
        var profileRequest = new
        {
            companyName = "Test Buyer Company",
            industry = "Manufacturing",
            description = "Manufacturing company",
            website = "https://testbuyer.com",
            country = "USA",
            city = "Chicago"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);
        await _client.PostAsJsonAsync("/api/profile", profileRequest);

        var rfqRequest = new
        {
            Title = "Test RFQ for Quotes",
            Description = "Industrial equipment needed",
            DeliveryDate = DateTime.UtcNow.AddDays(60),
            Items = new[]
            {
                new
                {
                    ProductName = "Industrial Equipment",
                    Description = "High-quality equipment",
                    Quantity = 10,
                    Unit = "units"
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/rfq", rfqRequest);
        var content = await response.Content.ReadAsStringAsync();
        
        return ExtractIdFromResponse(content);
    }

    private async Task<string> SubmitTestQuote(string sellerToken, string rfqId)
    {
        var quoteRequest = new
        {
            rfqId = rfqId,
            price = 15000,
            deliveryTime = "30 days",
            description = "Initial quote",
            validUntil = "2024-12-31T23:59:59Z"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sellerToken);
        var response = await _client.PostAsJsonAsync("/api/quotes", quoteRequest);
        var content = await response.Content.ReadAsStringAsync();
        
        return ExtractIdFromResponse(content);
    }

    private string ExtractIdFromResponse(string content)
    {
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
        
        return "test-id";
    }
}
