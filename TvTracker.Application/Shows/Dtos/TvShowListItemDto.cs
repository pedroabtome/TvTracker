namespace TvTracker.Application.Shows.Dtos;

public class TvShowListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Summary { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Network { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly? Premiered { get; set; }

    // nomes dos géneros para mostrar na lista
    public List<string> Genres { get; set; } = new();
}
