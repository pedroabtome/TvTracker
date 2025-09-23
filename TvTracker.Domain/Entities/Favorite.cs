namespace TvTracker.Domain.Entities;

public class Favorite
{
    public int UserId { get; set; }
    public int TvShowId { get; set; }

    public User User { get; set; } = null!;
    public TvShow TvShow { get; set; } = null!;
}
