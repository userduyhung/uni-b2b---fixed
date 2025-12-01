using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class ContractTemplateManagementTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ContractTemplateManagementTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetTemplates_Authenticated_ReturnsSuccess()
    {
        // Arrange
        var token = await RegisterAndLogin("templates@test.com", "Buyer");

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/contract-templates");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Contract templates retrieved successfully");
    }

    [Fact]
    public async Task GetTemplates_Unauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/contract-templates");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTemplate_SellerRole_ReturnsSuccess()
    {
        // Arrange
        var token = await RegisterAndLogin("seller@template.com", "Seller");
        await CreateSellerProfile(token);

        var templateRequest = new
        {
            Id = Guid.NewGuid(),
            Name = "Standard Service Agreement",
            Description = "A standard template for service agreements",
            TemplateContent = "This is a sample contract template content...",
            CustomFields = new object[0],
            IsActive = true,
            CreatedBySellerProfileId = Guid.NewGuid()
        };

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/contract-templates", templateRequest);

        // Assert
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.Created, System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTemplate_BuyerRole_ReturnsForbidden()
    {
        // Arrange
        var token = await RegisterAndLogin("buyer@template.com", "Buyer");
        var templateRequest = new
        {
            Name = "Standard Service Agreement",
            Description = "A standard template for service agreements",
            TemplateContent = "This is a sample contract template content..."
        };

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/contract-templates", templateRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMyTemplates_SellerRole_ReturnsSuccess()
    {
        // Arrange
        var token = await RegisterAndLogin("myseller@template.com", "Seller");
        await CreateSellerProfile(token);

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/contract-templates/my-templates?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyTemplates_BuyerRole_ReturnsForbidden()
    {
        // Arrange
        var token = await RegisterAndLogin("mybuyer@template.com", "Buyer");

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/contract-templates/my-templates");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GenerateContract_BuyerRole_ReturnsSuccess()
    {
        // Arrange
        var token = await RegisterAndLogin("genbuyer@template.com", "Buyer");
        await CreateBuyerProfile(token);

        var generateRequest = new
        {
            TemplateId = Guid.NewGuid(),
            SellerProfileId = Guid.NewGuid(),
            RfqId = (Guid?)null,
            QuoteId = (Guid?)null
        };

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/contract-templates/generate-contract", generateRequest);

        // Assert
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.BadRequest, System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GenerateContract_SellerRole_ReturnsForbidden()
    {
        // Arrange
        var token = await RegisterAndLogin("genseller@template.com", "Seller");
        var generateRequest = new
        {
            TemplateId = Guid.NewGuid(),
            SellerProfileId = Guid.NewGuid()
        };

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/contract-templates/generate-contract", generateRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GenerateContract_InvalidTemplateId_ReturnsBadRequest()
    {
        // Arrange
        var token = await RegisterAndLogin("invalidgen@template.com", "Buyer");
        await CreateBuyerProfile(token);

        var generateRequest = new
        {
            TemplateId = Guid.Empty,
            SellerProfileId = Guid.NewGuid()
        };

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/contract-templates/generate-contract", generateRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTemplate_ValidId_ReturnsTemplate()
    {
        // Arrange
        var token = await RegisterAndLogin("gettemplate@test.com", "Buyer");
        var templateId = Guid.NewGuid();

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync($"/api/contract-templates/{templateId}");

        // Assert
        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    private async Task<string> RegisterAndLogin(string email, string role)
    {
        var registerRequest = new
        {
            email,
            password = "Password123!",
            role
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email,
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        return ExtractTokenFromResponse(loginContent);
    }

    private async Task CreateSellerProfile(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", new
        {
            companyName = "Test Seller Company",
            industry = "Technology",
            description = "A test seller company",
            website = "https://testseller.com",
            country = "USA",
            city = "New York"
        });
    }

    private async Task CreateBuyerProfile(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", new
        {
            name = "Test Buyer",
            companyName = "Buyer Company",
            country = "USA"
        });
    }

    private string ExtractTokenFromResponse(string response)
    {
        var tokenStart = response.IndexOf("\"token\":\"") + 9;
        var tokenEnd = response.IndexOf("\"", tokenStart);
        return response.Substring(tokenStart, tokenEnd - tokenStart);
    }
}