using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using System.Net.Http.Headers;
using System.Text.Json;

namespace B2BMarketplace.Api.IntegrationTests;

public class ComprehensiveApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ComprehensiveApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task SellerWorkflow_CompleteFlow_ShouldWork()
    {
        // Register and login seller
        var sellerToken = await RegisterAndLogin("seller@test.com", "Seller");
        
        // Create seller profile
        var profileRequest = new
        {
            companyName = "Tech Solutions Inc",
            legalRepresentative = "John Smith",
            taxId = "123456789",
            industry = "Technology",
            country = "USA",
            description = "Leading provider of technology solutions"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sellerToken);
        var profileResponse = await _client.PostAsJsonAsync("/api/profile", profileRequest);
        profileResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Get seller profile
        var getProfileResponse = await _client.GetAsync("/api/profile");
        getProfileResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Update seller profile
        var updateRequest = new
        {
            companyName = "Tech Solutions Inc",
            legalRepresentative = "John Smith",
            taxId = "123456789",
            industry = "Technology",
            country = "USA",
            description = "Updated description - Leading provider of technology solutions and industrial equipment"
        };

        var updateResponse = await _client.PutAsJsonAsync("/api/profile", updateRequest);
        updateResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task BuyerWorkflow_CompleteFlow_ShouldWork()
    {
        // Register and login buyer
        var buyerToken = await RegisterAndLogin("buyer@test.com", "Buyer");
        
        // Update buyer profile
        var profileRequest = new
        {
            name = "John Buyer",
            companyName = "ABC Corporation",
            country = "USA",
            phone = "+1-555-0123"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);
        var profileResponse = await _client.PutAsJsonAsync("/api/profile", profileRequest);
        profileResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Create RFQ
        var rfqRequest = new
        {
            title = "Need 100 units of Industrial Fasteners",
            description = "Looking for high-quality industrial fasteners for manufacturing",
            deliveryDate = "2025-12-01T00:00:00Z",
            items = new[]
            {
                new { productName = "M10 Steel Bolts", quantity = 100, unit = "pieces" },
                new { productName = "M8 Steel Nuts", quantity = 200, unit = "pieces" }
            }
        };

        var rfqResponse = await _client.PostAsJsonAsync("/api/rfq", rfqRequest);
        rfqResponse.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.Created);

        // Get buyer RFQs
        var getRfqsResponse = await _client.GetAsync("/api/rfq/buyer?page=1&pageSize=10");
        getRfqsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchApis_ShouldWork()
    {
        // Search sellers by keyword
        var keywordResponse = await _client.GetAsync("/api/search/sellers?query=electronics&page=1&pageSize=10");
        keywordResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Search sellers by industry
        var industryResponse = await _client.GetAsync("/api/search/sellers?industry=Technology&page=1&pageSize=10");
        industryResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Search sellers by certification
        var certResponse = await _client.GetAsync("/api/search/sellers?certification=ISO&page=1&pageSize=10");
        certResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Search sellers by location
        var locationResponse = await _client.GetAsync("/api/search/sellers?location=USA&page=1&pageSize=10");
        locationResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task NotificationApis_ShouldWork()
    {
        var buyerToken = await RegisterAndLogin("notif_buyer@test.com", "Buyer");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);

        // Get notifications
        var notifResponse = await _client.GetAsync("/api/notifications?page=1&pageSize=20");
        notifResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // Update notification preferences
        var prefsRequest = new
        {
            emailNotificationsEnabled = true,
            rfqResponseNotifications = true,
            rfqReceivedNotifications = true,
            quoteUpdatedNotifications = true,
            certificationStatusNotifications = false,
            accountStatusNotifications = true
        };

        var prefsResponse = await _client.PutAsJsonAsync("/api/notifications/preferences", prefsRequest);
        prefsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReviewApis_ShouldWork()
    {
        var buyerToken = await RegisterAndLogin("review_buyer@test.com", "Buyer");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);

        // Get my reviews
        var myReviewsResponse = await _client.GetAsync("/api/reviews/my-reviews?page=1&pageSize=10");
        myReviewsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task PasswordChangeApi_ShouldWork()
    {
        var token = await RegisterAndLogin("pwd_change@test.com", "Buyer");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var changeRequest = new
        {
            currentPassword = "TestPass123!",
            newPassword = "NewTestPass123!"
        };

        var response = await _client.PutAsJsonAsync("/api/Auth/change-password", changeRequest);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    private async Task<string> RegisterAndLogin(string email, string role)
    {
        // Register
        var registerRequest = new
        {
            email = email,
            password = "TestPass123!",
            role = role
        };
        await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);

        // Login
        var loginRequest = new
        {
            email = email,
            password = "TestPass123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        
        using var doc = JsonDocument.Parse(loginContent);
        return doc.RootElement.GetProperty("data").GetProperty("token").GetString() ?? "";
    }
}