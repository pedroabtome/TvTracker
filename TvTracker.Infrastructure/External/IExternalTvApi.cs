using System.Collections.Generic;
using System.Threading.Tasks;

namespace TvTracker.Infrastructure.External;

public interface IExternalTvApi
{
    Task<IReadOnlyList<ExternalShowDto>> GetShowsAsync(int page, int pageSize);
    Task<IReadOnlyList<ExternalEpisodeDto>> GetEpisodesAsync(int externalShowId);
}
