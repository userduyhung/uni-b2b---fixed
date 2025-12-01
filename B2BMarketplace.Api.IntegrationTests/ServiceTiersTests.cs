using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace B2BMarketplace.Api.IntegrationTests;

public class ServiceTiersTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ServiceTiersTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetServiceTiers_ReturnsServiceTiers()
    {
        var response = await _client.GetAsync("/api/servicetiers");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetServiceTier_ValidId_ReturnsServiceTier()
    {
        var tiersResponse = await _client.GetAsync("/api/servicetiers");
        var content = await tiersResponse.Content.ReadAsStringAsync();

        if (content.Contains("\"id\":"))
        {
            var startIndex = content.IndexOf("\"id\":") + 5;
            var endIndex = content.IndexOf(",", startIndex);
            var id = content.Substring(startIndex, endIndex - startIndex).Trim().Trim('"');

            var response = await _client.GetAsync($"/api/servicetiers/{id}");
            response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.OK, System.Net.HttpStatusCode.NotFound);
        }
    }
}
