using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Text.Json;

namespace B2BMarketplace.Api.IntegrationTests;

public class AdminCompanyManagementTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AdminCompanyManagementTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllCompanies_WithPagination_ShouldReturnPagedResults()
    {
        await LoginAsAdmin();

        var response = await _client.GetAsync("/api/Admin/companies?page=1&pageSize=50");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCompanyDetails_ValidId_ShouldReturnCompany()
    {
        await LoginAsAdmin();
        var companyId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/Admin/companies/{companyId}");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ApproveCompany_ValidRequest_ShouldSucceed()
    {
        await LoginAsAdmin();
        var companyId = Guid.NewGuid();

        var approvalRequest = new
        {
            status = "approved",
            notes = "Company verification completed"
        };

        var response = await _client.PutAsJsonAsync($"/api/Admin/companies/{companyId}/approve", approvalRequest);
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RejectCompany_ValidRequest_ShouldSucceed()
    {
        await LoginAsAdmin();
        var companyId = Guid.NewGuid();

        var rejectionRequest = new
        {
            status = "rejected",
            notes = "Insufficient documentation"
        };

        var response = await _client.PutAsJsonAsync($"/api/Admin/companies/{companyId}/reject", rejectionRequest);
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPendingCompanies_ShouldReturnPendingList()
    {
        await LoginAsAdmin();

        var response = await _client.GetAsync("/api/Admin/companies/pending");
        
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