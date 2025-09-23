using FluentAssertions;

namespace TvTracker.Tests.Integration;

public class TvShowsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TvShowsEndpointsTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient();

    [Fact(Skip = "Desativar os integration test")]
    public async Task List_Returns200()
    {
        var res = await _client.GetAsync("/api/tvshows");
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
