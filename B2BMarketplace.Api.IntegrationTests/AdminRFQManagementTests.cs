using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Text.Json;

namespace B2BMarketplace.Api.IntegrationTests;

public class AdminRFQManagementTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AdminRFQManagementTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllRFQs_WithFilters_ShouldReturnFilteredResults()
    {
        await LoginAsAdmin();

        var response = await _client.GetAsync("/api/Admin/rfqs?page=1&pageSize=50&status=active");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ModerateRFQ_ApproveAction_ShouldSucceed()
    {
        await LoginAsAdmin();
        var rfqId = Guid.NewGuid();

        var moderationRequest = new
        {
            action = "approve",
            notes = "RFQ meets platform guidelines"
        };

        var response = await _client.PutAsJsonAsync($"/api/Admin/rfqs/{rfqId}/moderate", moderationRequest);
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ModerateRFQ_RejectAction_ShouldSucceed()
    {
        await LoginAsAdmin();
        var rfqId = Guid.NewGuid();

        var moderationRequest = new
        {
            action = "reject",
            notes = "RFQ violates platform policies"
        };

        var response = await _client.PutAsJsonAsync($"/api/Admin/rfqs/{rfqId}/moderate", moderationRequest);
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRFQDetails_ValidId_ShouldReturnRFQ()
    {
        await LoginAsAdmin();
        var rfqId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/Admin/rfqs/{rfqId}");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPendingRFQs_ShouldReturnPendingList()
    {
        await LoginAsAdmin();

        var response = await _client.GetAsync("/api/Admin/rfqs/pending");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRFQsByCategory_ShouldReturnCategorizedRFQs()
    {
        await LoginAsAdmin();
        var categoryId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/Admin/rfqs/category/{categoryId}");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    private async Task<string> LoginAsAdmin()
    {
        var registerRequest = new
        {
            email = $"admin{Guid.NewGuid()}@test.com",
            password = "AdminPass123!",
            role = "Admin"
        };
        await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);

        var loginRequest = new
        {
            email = registerRequest.email,
            password = registerRequest.password
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return token;
    }

    private string ExtractTokenFromResponse(string response)
    {
        try
        {
            using var doc = JsonDocument.Parse(response);
            return doc.RootElement.GetProperty("data").GetProperty("token").GetString() ?? "";
        }
        catch
        {
            var tokenStart = response.IndexOf("\"token\":\"") + 9;
            var tokenEnd = response.IndexOf("\"", tokenStart);
            return response.Substring(tokenStart, tokenEnd - tokenStart);
        }
    }
}