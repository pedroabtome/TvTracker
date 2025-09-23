namespace TvTracker.Application.Shows.Dtos;

public class TvShowDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Summary { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Network { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly? Premiered { get; set; }
    public DateTime LastUpdated { get; set; }

    public List<string> Genres { get; set; } = new();
    public List<EpisodeDto> Episodes { get; set; } = new();
    public List<ActorDto> Cast { get; set; } = new();
}
