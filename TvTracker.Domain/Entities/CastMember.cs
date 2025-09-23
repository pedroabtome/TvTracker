namespace TvTracker.Domain.Entities;

public class CastMember
{
    public int TvShowId { get; set; }
    public int ActorId { get; set; }
    public string? CharacterName { get; set; }

    public TvShow TvShow { get; set; } = null!;
    public Actor Actor { get; set; } = null!;
}
