namespace TvTracker.Infrastructure.Sync;

public class SyncOptions
{
    public int IntervalMinutes { get; set; } = 15;
    public int PageSize { get; set; } = 50;
    public int StartPage { get; set; } = 1;
}
