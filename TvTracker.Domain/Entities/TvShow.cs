namespace TvTracker.Domain.Entities;

public class TvShow
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Summary { get; set; }
    public string? Type { get; set; }      // ex.: Scripted, Reality…
    public string? Status { get; set; }    // ex.: Running, Ended…
    public string? Network { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly? Premiered { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Relações
    public List<Episode> Episodes { get; set; } = new();
    public List<Genre> Genres { get; set; } = new();          // m:n (EF cria join table)
    public List<CastMember> Cast { get; set; } = new();       // m:n via entidade de junção
    public List<Favorite> Favorites { get; set; } = new();    // users que favoritaram


    public int? ExternalId { get; set; }   // <- novo


}
