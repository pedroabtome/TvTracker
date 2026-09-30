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


/*
    private async Task RunOnce(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TrackerDbContext>();

        // 1) pagina de shows externos
        for (int page = _opt.StartPage; page < _opt.StartPage + _opt.MaxPages; page++)
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

*/

private async Task RunOnce(CancellationToken ct)
{
    using var scope = _sp.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TrackerDbContext>();

    for (int page = _opt.StartPage;
         page < _opt.StartPage + _opt.MaxPages;
         page++)
    {
        var shows = await _external.GetShowsAsync(page, _opt.PageSize, ct);

        if (shows.Count == 0)
            break;

        foreach (var item in shows)
        {
            var details = await _external.GetShowDetailsAsync(item.Id, ct);

            if (details is null)
                continue;

            var show = await db.TvShows
                .Include(s => s.Genres)
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(
                    s => s.ExternalId == details.Id ||
                         (s.ExternalId == null && s.Name == details.Name),
                    ct);

            if (show is null)
            {
                show = new TvShow
                {
                    ExternalId = details.Id
                };

                db.TvShows.Add(show);
            }
            else if (show.ExternalId is null)
            {
                show.ExternalId = details.Id;
            }

            show.Name = details.Name;
            show.Summary = details.Summary;
            show.Type = details.Type;
            show.Status = details.Status;
            show.Network = details.Network;
            show.ImageUrl = details.ImageUrl;
            show.Premiered = details.Premiered;
            show.LastUpdated = DateTime.UtcNow;

            show.Genres.Clear();

            foreach (var genreName in details.Genres.Distinct())
            {
                var genre = await db.Genres
                    .FirstOrDefaultAsync(g => g.Name == genreName, ct);

                if (genre is null)
                {
                    genre = new Genre
                    {
                        Name = genreName
                    };

                    db.Genres.Add(genre);
                }

                show.Genres.Add(genre);
            }

            foreach (var itemEpisode in details.Episodes)
            {
                var episode = show.Episodes.FirstOrDefault(e =>
                    e.Season == itemEpisode.Season &&
                    e.Number == itemEpisode.Number);

                if (episode is null)
                {
                    episode = new Episode
                    {
                        TvShow = show,
                        Season = itemEpisode.Season,
                        Number = itemEpisode.Number
                    };

                    show.Episodes.Add(episode);
                }

                episode.Name = itemEpisode.Name;
                episode.AirDate = itemEpisode.AirDate;
                episode.AirTime = itemEpisode.AirTime;
                episode.Runtime = itemEpisode.Runtime;
                episode.Summary = itemEpisode.Summary;
            }

            await db.SaveChangesAsync(ct);
        }

        _log.LogInformation(
            "Synced page {page} with {count} shows",
            page,
            shows.Count);
    }
}

}
