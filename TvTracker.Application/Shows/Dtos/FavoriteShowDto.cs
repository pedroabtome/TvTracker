namespace TvTracker.Application.Shows.Dtos;

public class FavoriteShowDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();
}
