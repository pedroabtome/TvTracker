namespace TvTracker.Application.Shows.Dtos;

public class EpisodeDto
{
    public int Id { get; set; }
    public int Season { get; set; }
    public int Number { get; set; }
    public DateOnly? AirDate { get; set; }
    public string? AirTime { get; set; }
    public int? Runtime { get; set; }
    public string? Name { get; set; }
    public string? Summary { get; set; }
}
