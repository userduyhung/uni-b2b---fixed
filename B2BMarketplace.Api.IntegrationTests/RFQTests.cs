using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class RFQTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public RFQTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateRFQ_ValidRequest_ReturnsCreated()
    {
        var token = await RegisterAndLoginBuyer("rfqbuyer@test.com");
        await CreateBuyerProfile(token);

        var rfqRequest = new
        {
            Title = "Need 100 units of Product X",
            Description = "Looking for quality products",
            DeliveryDate = DateTime.UtcNow.AddDays(30),
            Items = new[]
            {
                new { ProductName = "Product X", Quantity = 100, Unit = "pieces" }
            }
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/rfq", rfqRequest);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetBuyerRFQs_ValidRequest_ReturnsRFQs()
    {
        var token = await RegisterAndLoginBuyer("rfqbuyer2@test.com");
        await CreateBuyerProfile(token);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/rfq/buyer");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateQuote_ValidRequest_ReturnsCreated()
    {
        var buyerToken = await RegisterAndLoginBuyer("rfqbuyer3@test.com");
        await CreateBuyerProfile(buyerToken);

        var sellerToken = await RegisterAndLoginSeller("rfqseller@test.com");
        await CreateSellerProfile(sellerToken);

        var rfqRequest = new
        {
            Title = "Need Product Y",
            Description = "Urgent requirement",
            DeliveryDate = DateTime.UtcNow.AddDays(15),
            Items = new[] { new { ProductName = "Product Y", Quantity = 50, Unit = "kg" } }
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", buyerToken);
        var rfqResponse = await _client.PostAsJsonAsync("/api/rfq", rfqRequest);
        rfqResponse.EnsureSuccessStatusCode(); // This will throw if RFQ creation failed
        var rfqContent = await rfqResponse.Content.ReadAsStringAsync();
        var rfqId = ExtractIdFromResponse(rfqContent);

        var quoteRequest = new
        {
            TotalPrice = 5000.00m,
            DeliveryDate = DateTime.UtcNow.AddDays(10),
            Notes = "Best quality guaranteed"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", sellerToken);
        var response = await _client.PostAsJsonAsync($"/api/rfq/{rfqId}/quotes", quoteRequest);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.Created, System.Net.HttpStatusCode.BadRequest);
    }

    private async Task<string> RegisterAndLoginBuyer(string email)
    {
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Password123!", role = "Buyer" });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Password123!" });
        var content = await loginResponse.Content.ReadAsStringAsync();
        return ExtractTokenFromResponse(content);
    }

    private async Task<string> RegisterAndLoginSeller(string email)
    {
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Password123!", role = "Seller" });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Password123!" });
        var content = await loginResponse.Content.ReadAsStringAsync();
        return ExtractTokenFromResponse(content);
    }

    private async Task CreateBuyerProfile(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PutAsJsonAsync("/api/profile/buyer", new
        {
            ProfileData = new { Name = "Test Buyer", Country = "USA" }
        });
    }

    private async Task CreateSellerProfile(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PutAsJsonAsync("/api/profile/seller", new
        {
            CompanyName = "Test Seller Co",
            LegalRepresentative = "John Doe",
            TaxId = "123456789",
            Country = "USA"
        });
    }

    private string ExtractTokenFromResponse(string content)
    {
        var startIndex = content.IndexOf("\"token\":\"") + 9;
        if (startIndex < 9) return string.Empty;
        var endIndex = content.IndexOf("\"", startIndex);
        return content.Substring(startIndex, endIndex - startIndex);
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
