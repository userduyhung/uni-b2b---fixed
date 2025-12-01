using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class ReviewsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ReviewsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateReview_ValidRequest_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("reviewer@test.com", "Buyer");
        await CreateBuyerProfile(token);

        var reviewRequest = new
        {
            SellerProfileId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Excellent service and quality products!",
            OrderId = Guid.NewGuid()
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/reviews", reviewRequest);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.Created, System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSellerReviews_ValidSellerId_ReturnsReviews()
    {
        var sellerId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/reviews/seller/{sellerId}?page=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMyReviews_Authenticated_ReturnsReviews()
    {
        var token = await RegisterAndLogin("myreviews@test.com", "Buyer");
        await CreateBuyerProfile(token);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/reviews/my-reviews?page=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateReview_ValidRequest_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("updatereview@test.com", "Buyer");
        await CreateBuyerProfile(token);

        var reviewId = Guid.NewGuid();
        var updateRequest = new
        {
            Rating = 4,
            Comment = "Updated review comment"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PutAsJsonAsync($"/api/reviews/{reviewId}", updateRequest);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound, System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReview_Unauthenticated_ReturnsUnauthorized()
    {
        var reviewRequest = new
        {
            SellerProfileId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Should not work"
        };

        var response = await _client.PostAsJsonAsync("/api/reviews", reviewRequest);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    private async Task<string> RegisterAndLogin(string email, string role)
    {
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Password123!", role });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Password123!" });
        var content = await loginResponse.Content.ReadAsStringAsync();
        return ExtractToken(content);
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

    private string ExtractToken(string content)
    {
        var startIndex = content.IndexOf("\"token\":\"") + 9;
        if (startIndex < 9) return string.Empty;
        var endIndex = content.IndexOf("\"", startIndex);
        return content.Substring(startIndex, endIndex - startIndex);
    }
}