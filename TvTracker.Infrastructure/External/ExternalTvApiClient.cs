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
