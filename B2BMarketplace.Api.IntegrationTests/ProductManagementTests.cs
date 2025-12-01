using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class ProductManagementTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductManagementTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_ValidRequest_ReturnsSuccess()
    {
        // First register and login a seller
        var registerRequest = new
        {
            email = "productseller@example.com",
            password = "Password123!",
            role = "Seller"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "productseller@example.com",
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        // Create seller profile
        var profileRequest = new
        {
            companyName = "Test Company",
            industry = "Electronics",
            description = "A test company description",
            website = "https://testcompany.com",
            country = "USA",
            city = "New York"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", profileRequest);

        // Arrange
        var productRequest = new
        {
            Name = "Test Product",
            Description = "A test product description",
            Category = "Electronics",
            ReferencePrice = 99.99
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", productRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Name");
        content.Should().Contain("Test Product");
    }

    [Fact]
    public async Task UpdateProduct_ValidRequest_ReturnsSuccess()
    {
        // First create a product
        var registerRequest = new
        {
            email = "updateseller@example.com",
            password = "Password123!",
            role = "Seller"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "updateseller@example.com",
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        // Create seller profile
        var profileRequest = new
        {
            companyName = "Test Company",
            industry = "Electronics",
            description = "A test company description",
            website = "https://testcompany.com",
            country = "USA",
            city = "New York"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", profileRequest);

        var productRequest = new
        {
            Name = "Update Test Product",
            Description = "A test product for updating",
            Category = "Electronics",
            ReferencePrice = 79.99
        };

        var createResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();

        // Extract product ID
        var idStart = createContent.IndexOf("\"Id\":") + 5;
        var idEnd = createContent.IndexOf(",", idStart);
        var productId = createContent.Substring(idStart, idEnd - idStart).Trim('"');

        // Arrange update request
        var updateRequest = new
        {
            Name = "Updated Product Name",
            Description = "Updated description",
            Category = "Electronics",
            ReferencePrice = 89.99
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/products/{productId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Updated Product Name");
    }

    [Fact]
    public async Task GetProduct_ValidId_ReturnsProduct()
    {
        // Setup seller and product
        var registerRequest = new
        {
            email = "getseller@example.com",
            password = "Password123!",
            role = "Seller"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "getseller@example.com",
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        var profileRequest = new
        {
            companyName = "Test Company",
            industry = "Electronics",
            description = "A test company description",
            website = "https://testcompany.com",
            country = "USA",
            city = "New York"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", profileRequest);

        var productRequest = new
        {
            Name = "Get Test Product",
            Description = "A test product for retrieval",
            Category = "Electronics",
            ReferencePrice = 59.99
        };

        var createResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var productId = ExtractIdFromResponse(createContent);

        // Act
        var response = await _client.GetAsync($"/api/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Get Test Product");
    }

    [Fact]
    public async Task GetProducts_ReturnsProductList()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DeleteProduct_ValidId_ReturnsSuccess()
    {
        // Setup seller and product
        var registerRequest = new
        {
            email = "deleteseller@example.com",
            password = "Password123!",
            role = "Seller"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "deleteseller@example.com",
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        var profileRequest = new
        {
            companyName = "Test Company",
            industry = "Electronics",
            description = "A test company description",
            website = "https://testcompany.com",
            country = "USA",
            city = "New York"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", profileRequest);

        var productRequest = new
        {
            Name = "Delete Test Product",
            Description = "A test product for deletion",
            Category = "Electronics",
            ReferencePrice = 39.99
        };

        var createResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var productId = ExtractIdFromResponse(createContent);

        // Act
        var response = await _client.DeleteAsync($"/api/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
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
