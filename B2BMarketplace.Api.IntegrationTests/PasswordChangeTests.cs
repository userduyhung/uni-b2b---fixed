using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class PasswordChangeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PasswordChangeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ChangePassword_ValidRequest_ReturnsSuccess()
    {
        // First register and login a user
        var registerRequest = new
        {
            email = "changepass@example.com",
            password = "OldPassword123!",
            role = "Buyer"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "changepass@example.com",
            password = "OldPassword123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        // Arrange
        var changePasswordRequest = new
        {
            currentPassword = "OldPassword123!",
            newPassword = "NewPassword123!",
            confirmNewPassword = "NewPassword123!"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsJsonAsync("/api/auth/change-password", changePasswordRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Password changed successfully");
    }

    [Fact]
    public async Task ChangePassword_IncorrectCurrentPassword_ReturnsBadRequest()
    {
        // First register and login a user
        var registerRequest = new
        {
            email = "wrongpass@example.com",
            password = "CorrectPassword123!",
            role = "Buyer"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "wrongpass@example.com",
            password = "CorrectPassword123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        // Arrange
        var changePasswordRequest = new
        {
            currentPassword = "WrongPassword123!",
            newPassword = "NewPassword123!",
            confirmNewPassword = "NewPassword123!"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsJsonAsync("/api/auth/change-password", changePasswordRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid current password");
    }

    [Fact]
    public async Task ChangePassword_PasswordsDontMatch_ReturnsBadRequest()
    {
        // First register and login a user
        var registerRequest = new
        {
            email = "mismatch@example.com",
            password = "Password123!",
            role = "Buyer"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "mismatch@example.com",
            password = "Password123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        // Arrange
        var changePasswordRequest = new
        {
            currentPassword = "Password123!",
            newPassword = "NewPassword123!",
            confirmNewPassword = "DifferentPassword123!"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsJsonAsync("/api/auth/change-password", changePasswordRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Passwords do not match");
    }

    [Fact]
    public async Task ChangePassword_WeakNewPassword_ReturnsBadRequest()
    {
        // First register and login a user
        var registerRequest = new
        {
            email = "weakpass@example.com",
            password = "StrongPassword123!",
            role = "Buyer"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "weakpass@example.com",
            password = "StrongPassword123!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = ExtractTokenFromResponse(loginContent);

        // Arrange
        var changePasswordRequest = new
        {
            currentPassword = "StrongPassword123!",
            newPassword = "123", // Too short
            confirmNewPassword = "123"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsJsonAsync("/api/auth/change-password", changePasswordRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Password must be at least 8 characters long");
    }

    [Fact]
    public async Task ChangePassword_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        var changePasswordRequest = new
        {
            currentPassword = "SomePassword123!",
            newPassword = "NewPassword123!",
            confirmNewPassword = "NewPassword123!"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/auth/change-password", changePasswordRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    private string ExtractTokenFromResponse(string responseContent)
    {
        // Simple extraction - in real implementation, use JSON deserialization
        var startIndex = responseContent.IndexOf("\"token\":\"") + 9;
        var endIndex = responseContent.IndexOf("\"", startIndex);
        return responseContent.Substring(startIndex, endIndex - startIndex);
    }
}
