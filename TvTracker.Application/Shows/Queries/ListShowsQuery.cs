using System.ComponentModel.DataAnnotations;

namespace TvTracker.Application.Shows.Queries;

// mapeia os parâmetros de querystring (?genre=&type=&sort=&order=&page=&pageSize=)
public class ListShowsQuery
{
    public string? Genre { get; set; }
    public string? Type  { get; set; } // e.g. "Scripted", "Reality"
    public string Sort   { get; set; } = "name"; // coluna segura (whitelist no serviço)
    public string Order  { get; set; } = "asc";  // asc|desc

    [Range(1, int.MaxValue)] public int Page     { get; set; } = 1;
    [Range(1, 200)]          public int PageSize { get; set; } = 20;
}
