namespace TvTracker.Domain.Entities;

public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<CastMember> CastIn { get; set; } = new();
}
