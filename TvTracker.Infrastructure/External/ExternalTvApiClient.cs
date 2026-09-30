/*

using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace TvTracker.Infrastructure.External;

public class ExternalTvApiClient : IExternalTvApi
{
    private readonly HttpClient _http;
    private readonly int _pageSize;

    public ExternalTvApiClient(HttpClient http, IConfiguration cfg)
    {
        _http = http;
        _pageSize = cfg.GetValue<int>("ExternalApi:PageSize", 50);
    }

    // ExternalTvApiClient.cs
public Task<IReadOnlyList<ExternalShowDto>> GetShowsAsync(int page, int pageSize)
    => Task.FromResult<IReadOnlyList<ExternalShowDto>>(Array.Empty<ExternalShowDto>());

public Task<IReadOnlyList<ExternalEpisodeDto>> GetEpisodesAsync(int externalShowId)
    => Task.FromResult<IReadOnlyList<ExternalEpisodeDto>>(Array.Empty<ExternalEpisodeDto>());

    
}

*/


using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace TvTracker.Infrastructure.External;

public class ExternalTvApiClient : IExternalTvApi
{
    private readonly HttpClient _http;

    public ExternalTvApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ExternalShowSummaryDto>> GetShowsAsync(
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var response = await _http.GetFromJsonAsync<PopularShowsResponse>(
            $"most-popular?page={page}", ct);

        if (response is null)
            return Array.Empty<ExternalShowSummaryDto>();

        return response.TvShows
            .Take(pageSize)
            .Select(show => new ExternalShowSummaryDto(show.Id, show.Name))
            .ToList();
    }

    public async Task<ExternalShowDto?> GetShowDetailsAsync(
        int externalShowId,
        CancellationToken ct = default)
    {
        var response = await _http.GetFromJsonAsync<ShowDetailsResponse>(
            $"show-details?q={externalShowId}", ct);

        var show = response?.TvShow;

        if (show is null)
            return null;

        var episodes = show.Episodes
            .Select(episode =>
            {
                var airDate = ParseDateTime(episode.AirDate);

                return new ExternalEpisodeDto(
                    episode.Season,
                    episode.Episode,
                    airDate.HasValue
                        ? DateOnly.FromDateTime(airDate.Value)
                        : null,
                    airDate.HasValue
                        ? TimeOnly.FromDateTime(airDate.Value)
                        : null,
                    show.Runtime,
                    episode.Name ?? $"Episode {episode.Episode}",
                    null);
            })
            .ToList();

        return new ExternalShowDto(
            show.Id,
            show.Name,
            show.Description,
            null,
            show.Status,
            show.Network,
            show.ImagePath,
            ParseDate(show.StartDate),
            show.Genres,
            episodes);
    }

    private static DateOnly? ParseDate(string? value)
    {
        return DateOnly.TryParse(
            value,
            CultureInfo.InvariantCulture,
            out var date)
            ? date
            : null;
    }

    private static DateTime? ParseDateTime(string? value)
    {
        return DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date)
            ? date
            : null;
    }

    private sealed class PopularShowsResponse
    {
        [JsonPropertyName("tv_shows")]
        public List<PopularShow> TvShows { get; set; } = new();
    }

    private sealed class PopularShow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    private sealed class ShowDetailsResponse
    {
        [JsonPropertyName("tvShow")]
        public ShowDetails? TvShow { get; set; }
    }

    private sealed class ShowDetails
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }

        [JsonPropertyName("start_date")]
        public string? StartDate { get; set; }

        public string? Status { get; set; }
        public int? Runtime { get; set; }
        public string? Network { get; set; }

        [JsonPropertyName("image_path")]
        public string? ImagePath { get; set; }

        public List<string> Genres { get; set; } = new();
        public List<EpisodeDetails> Episodes { get; set; } = new();
    }

    private sealed class EpisodeDetails
    {
        public int Season { get; set; }
        public int Episode { get; set; }
        public string? Name { get; set; }

        [JsonPropertyName("air_date")]
        public string? AirDate { get; set; }
    }
}