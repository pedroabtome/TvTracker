using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TvTracker.Domain.Entities;

public class Episode
{
    public int Id { get; set; }

    // FK para TvShow
    public int TvShowId { get; set; }
    public TvShow TvShow { get; set; } = default!;

    // Identificação dentro da série
    public int Season { get; set; }
    public int Number { get; set; }

    // Metadados
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateOnly? AirDate { get; set; }
    public TimeOnly? AirTime { get; set; }
    public int? Runtime { get; set; }

    // NOVO: rating IMDb (0..10). Pode vir null se não houver.
    [Column(TypeName = "decimal(3,1)")]
    public decimal? ImdbRating { get; set; }

    // (se tiveres Summary/Synopsis no teu projeto, podes manter)
    public string? Summary { get; set; }

    public int? ExternalId { get; set;}

}
