using FluentAssertions;

namespace TvTracker.Tests.Integration;

public class FavoritesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FavoritesEndpointsTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient();

    [Fact(Skip = "Desativar os integration test")]
    public async Task ListFavorites_Returns200()
    {
        var res = await _client.GetAsync("/api/me/favorites");
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
