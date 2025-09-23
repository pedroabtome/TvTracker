using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TvTracker.Infrastructure.Persistence;

public class TrackerDbContextFactory : IDesignTimeDbContextFactory<TrackerDbContext>
{
    public TrackerDbContext CreateDbContext(string[] args)
    {
        // 1) permite override via variável de ambiente
        var conn = Environment.GetEnvironmentVariable("TVTRACKER_CONN")
                   ?? "Host=localhost;Port=5432;Database=tvtracker;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<TrackerDbContext>()
            .UseNpgsql(conn)
            .Options;

        return new TrackerDbContext(options);
    }
}
