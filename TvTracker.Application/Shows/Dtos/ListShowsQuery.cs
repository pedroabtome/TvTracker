namespace TvTracker.Application.Shows;

public class ListShowsQuery
{
    // filtros
    public string? Search { get; set; }   // procura por nome (ILIKE)
    public string? Genre  { get; set; }
    public string? Type   { get; set; }
    public string? Status { get; set; }

    // ordenação dinâmica: "name" ou "-premiered"
    public string? Sort { get; set; }

    // paginação
    public int Page { get; set; } = 1;
    private int _pageSize = 20;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > 100 ? 20 : value;
    }
}

