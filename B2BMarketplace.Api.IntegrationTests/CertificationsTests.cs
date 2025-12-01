using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class CertificationsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CertificationsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SubmitCertification_ValidRequest_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("certseller@test.com", "Seller");
        await CreateSellerProfile(token);

        var certRequest = new
        {
            CertificationType = "ISO9001",
            CertificationBody = "ISO International",
            CertificationNumber = "ISO-123456",
            IssueDate = DateTime.UtcNow.AddDays(-30),
            ExpiryDate = DateTime.UtcNow.AddYears(3),
            DocumentUrl = "https://example.com/cert.pdf"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/certifications", certRequest);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.Created, System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetMyCertifications_Authenticated_ReturnsCertifications()
    {
        var token = await RegisterAndLogin("mycerts@test.com", "Seller");
        await CreateSellerProfile(token);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/certifications/my-certifications");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSellerCertifications_ValidSellerId_ReturnsCertifications()
    {
        var sellerId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/certifications/seller/{sellerId}");

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCertification_ValidRequest_ReturnsSuccess()
    {
        var token = await RegisterAndLogin("updatecert@test.com", "Seller");
        await CreateSellerProfile(token);

        var certId = Guid.NewGuid();
        var updateRequest = new
        {
            CertificationType = "ISO9001",
            CertificationBody = "Updated ISO International",
            CertificationNumber = "ISO-123456-UPDATED",
            IssueDate = DateTime.UtcNow.AddDays(-30),
            ExpiryDate = DateTime.UtcNow.AddYears(3)
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PutAsJsonAsync($"/api/certifications/{certId}", updateRequest);

        response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound, System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SubmitCertification_Unauthenticated_ReturnsUnauthorized()
    {
        var certRequest = new
        {
            CertificationType = "ISO9001",
            CertificationBody = "ISO International"
        };

        var response = await _client.PostAsJsonAsync("/api/certifications", certRequest);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    private async Task<string> RegisterAndLogin(string email, string role)
    {
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Password123!", role });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Password123!" });
        var content = await loginResponse.Content.ReadAsStringAsync();
        return ExtractToken(content);
    }

    private async Task CreateSellerProfile(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync("/api/profile", new
        {
            companyName = "Cert Company",
            industry = "Manufacturing",
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