using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class ProfileManagementTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProfileManagementTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateSellerProfile_ValidRequest_ReturnsSuccess()
    {
        var token = await RegisterAndLoginSeller("seller1@example.com");

        var request = new
        {
            companyName = "Test Company",
            industry = "Technology",
            description = "A technology company",
            website = "https://testcompany.com",
            country = "USA",
            city = "New York"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/profile", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Test Company");
    }

    [Fact]
    public async Task CreateBuyerProfile_ValidRequest_ReturnsSuccess()
    {
        var token = await RegisterAndLoginBuyer("buyer1@example.com");

        var request = new
        {
            companyName = "Buyer Company",
            industry = "Retail",
            description = "A retail company",
            website = "https://buyercompany.com",
            country = "Canada",
            city = "Toronto"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/profile", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProfile_AuthenticatedUser_ReturnsProfile()
    {
        var token = await RegisterAndLoginSeller("getprofile@example.com");

        var createRequest = new
        {
            companyName = "Profile Company",
            industry = "Manufacturing",
            description = "A manufacturing company",
            website = "https://profilecompany.com",
            country = "Germany",
            city = "Berlin"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", createRequest);

        var response = await _client.GetAsync("/api/profile");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Profile Company");
    }

    [Fact]
    public async Task UpdateProfile_ValidRequest_ReturnsSuccess()
    {
        var token = await RegisterAndLoginSeller("updateprofile@example.com");

        var createRequest = new
        {
            companyName = "Original Company",
            industry = "Technology",
            description = "Original description",
            website = "https://original.com",
            country = "USA",
            city = "San Francisco"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", createRequest);

        var updateRequest = new
        {
            companyName = "Updated Company",
            industry = "Technology",
            description = "Updated description",
            website = "https://updated.com",
            country = "USA",
            city = "Los Angeles"
        };

        var response = await _client.PutAsJsonAsync("/api/profile", updateRequest);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Updated Company");
    }

    [Fact]
    public async Task CreateProfile_Unauthenticated_ReturnsUnauthorized()
    {
        var request = new
        {
            companyName = "Unauthorized Company",
            industry = "Technology",
            description = "Should not work",
            website = "https://unauthorized.com",
            country = "USA",
            city = "New York"
        };

        var response = await _client.PostAsJsonAsync("/api/profile", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
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
}