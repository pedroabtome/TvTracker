using Microsoft.Extensions.DependencyInjection;  // <- para CreateScope()
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TvTracker.Domain.Entities;
using TvTracker.Infrastructure.External;
using TvTracker.Infrastructure.Persistence;

namespace TvTracker.Infrastructure.Sync;

public class EpisodeSyncWorker : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly IExternalTvApi _external;
    private readonly SyncOptions _opt;
    private readonly ILogger<EpisodeSyncWorker> _log;

    public EpisodeSyncWorker(IServiceProvider sp, IExternalTvApi external, IOptions<SyncOptions> opt, ILogger<EpisodeSyncWorker> log)
    {
        _sp = sp; _external = external; _opt = opt.Value; _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _log.LogInformation("EpisodeSyncWorker started. Interval: {m} min", _opt.IntervalMinutes);
        while (!ct.IsCancellationRequested)
        {
            try { await RunOnce(ct); }
            catch (Exception ex) { _log.LogError(ex, "Sync error"); }

            await Task.Delay(TimeSpan.FromMinutes(_opt.IntervalMinutes), ct);
        }
    }

    private async Task RunOnce(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TrackerDbContext>();

        // 1) pagina de shows externos
        for (int page = _opt.StartPage; ; page++)
        {
            var shows = await _external.GetShowsAsync(page, _opt.PageSize);
            if (shows.Count == 0) break;

            foreach (var s in shows)
            {
                // UPSERT de TvShow por ExternalId
                var show = await db.TvShows.FirstOrDefaultAsync(x => x.ExternalId == s.Id, ct);
                if (show is null)
                {
                    show = new TvShow { ExternalId = s.Id };
                    db.TvShows.Add(show);
                }

                show.Name = s.Name;
                show.Summary = s.Summary;
                show.Type = s.Type;
                show.Status = s.Status;
                show.Network = s.Network;
                show.ImageUrl = s.ImageUrl;
                show.Premiered = s.Premiered;
                show.LastUpdated = DateTime.UtcNow;

                // 2) episódios do show (upsert por ExternalId)
                var eps = await _external.GetEpisodesAsync(s.Id);
                foreach (var e in eps)
                {
                    var ep = await db.Episodes.FirstOrDefaultAsync(x => x.TvShowId == show.Id && x.ExternalId == e.Id, ct);
                    if (ep is null)
                    {
                        ep = new Episode { TvShow = show, ExternalId = e.Id };
                        db.Episodes.Add(ep);
                    }

                    ep.Season = e.Season;
                    ep.Number = e.Number;
                    ep.AirDate = e.AirDate;
                    ep.AirTime = e.AirTime;
                    ep.Runtime = e.Runtime;
                    ep.Name = e.Name;
                    ep.Summary = e.Summary;
                }
            }

            await db.SaveChangesAsync(ct);
            _log.LogInformation("Synced page {page} with {count} shows", page, shows.Count);
        }
    }
}
