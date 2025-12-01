using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using System.Net.Http.Headers;
using System.Text.Json;

namespace B2BMarketplace.Api.IntegrationTests;

public class AdminApiIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AdminApiIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task AdminDashboard_ShouldWork()
    {
        var adminToken = await LoginAdmin();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Get dashboard stats
        var statsResponse = await _client.GetAsync("/api/Admin/dashboard/stats");
        statsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Get analytics overview
        var analyticsResponse = await _client.GetAsync("/api/Admin/analytics");
        analyticsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Get user growth analytics
        var userGrowthResponse = await _client.GetAsync("/api/Admin/analytics/user-growth?startDate=2025-01-01&endDate=2025-12-31&interval=daily");
        userGrowthResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Get RFQ analytics
        var rfqAnalyticsResponse = await _client.GetAsync("/api/Admin/analytics/rfq?startDate=2025-01-01&endDate=2025-12-31&interval=daily");
        rfqAnalyticsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminUserManagement_ShouldWork()
    {
        var adminToken = await LoginAdmin();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Get all users
        var usersResponse = await _client.GetAsync("/api/Admin/users?page=1&pageSize=50");
        usersResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Search users
        var searchResponse = await _client.GetAsync("/api/Admin/users/search?query=john&userType=buyer&status=active");
        searchResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminCompanyManagement_ShouldWork()
    {
        var adminToken = await LoginAdmin();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Get all companies
        var companiesResponse = await _client.GetAsync("/api/Admin/companies?page=1&pageSize=50");
        companiesResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminRFQManagement_ShouldWork()
    {
        var adminToken = await LoginAdmin();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Get all RFQs
        var rfqsResponse = await _client.GetAsync("/api/Admin/rfqs?page=1&pageSize=50&status=active");
        rfqsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminSystemManagement_ShouldWork()
    {
        var adminToken = await LoginAdmin();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Get system health
        var healthResponse = await _client.GetAsync("/api/Admin/system/health");
        healthResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Get audit logs
        var auditResponse = await _client.GetAsync("/api/Admin/audit-logs?page=1&pageSize=100&startDate=2025-01-01&endDate=2025-12-31");
        auditResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Update system settings
        var settingsRequest = new
        {
            maintenanceMode = false,
            maxRfqsPerDay = 100,
            autoApprovalThreshold = 10000
        };

        var settingsResponse = await _client.PutAsJsonAsync("/api/Admin/system/settings", settingsRequest);
        settingsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminCertificationManagement_ShouldWork()
    {
        var adminToken = await LoginAdmin();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Get all certifications
        var certsResponse = await _client.GetAsync("/api/admin/certifications?page=1&size=10");
        certsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Search certifications
        var searchResponse = await _client.GetAsync("/api/admin/certifications/search?searchTerm=ISO&page=1&size=10");
        searchResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminCategoryManagement_ShouldWork()
    {
        var adminToken = await LoginAdmin();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Get all categories
        var categoriesResponse = await _client.GetAsync("/api/admin/categories");
        categoriesResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Create category
        var createRequest = new
        {
            Name = "Electronics",
            Description = "Electronic components and devices",
            Slug = "electronics",
            DisplayOrder = 1,
            IsActive = true
        };

        var createResponse = await _client.PostAsJsonAsync("/api/admin/categories", createRequest);
        createResponse.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task AdminContentModeration_ShouldWork()
    {
        var adminToken = await LoginAdmin();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Get content items
        var contentResponse = await _client.GetAsync("/api/admin/ContentManagement/items?page=1&pageSize=10");
        contentResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminAuditLogs_ShouldWork()
    {
        var adminToken = await LoginAdmin();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Get audit logs
        var logsResponse = await _client.GetAsync("/api/admin/audit-logs?page=1&pageSize=50");
        logsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    private async Task<string> LoginAdmin()
    {
        var loginRequest = new
        {
            email = "admin@b2bmarketplace.com",
            password = "AdminPass123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        
        using var doc = JsonDocument.Parse(loginContent);
        return doc.RootElement.GetProperty("data").GetProperty("token").GetString() ?? "";
    }
}