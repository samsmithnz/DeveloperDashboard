using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeveloperDashboard.Web.Tests;

public class ReadinessEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ReadinessEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ReadyEndpointReportsApplicationReadiness()
    {
        var response = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task DashboardIdentifiesItsDataAsFictionalPrototypeData()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Fictional data only", await response.Content.ReadAsStringAsync());
    }
}
