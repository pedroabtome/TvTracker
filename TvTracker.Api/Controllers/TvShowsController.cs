using Microsoft.AspNetCore.Mvc;
using TvTracker.Application.Shows;
using TvTracker.Application.Shows.Dtos;

namespace TvTracker.Api.Controllers;

[ApiController]
[Route("api/tvshows")]
public class TvShowsController : ControllerBase
{
    private readonly IShowService _service;

    public TvShowsController(IShowService service) => _service = service;

    // GET /api/tvshows?genre=&type=&status=&search=&sort=name|-premiered&order=asc|desc&page=1&pageSize=20
    [HttpGet]
    public async Task<ActionResult> ListAsync([FromQuery] ListShowsQuery q, CancellationToken ct)
    {
        var result = await _service.ListAsync(q, ct);
        return Ok(result); // { items, page, pageSize, total }
    }

    // GET /api/tvshows/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TvShowDetailDto>> GetByIdAsync([FromRoute] int id, CancellationToken ct)
    {
        var show = await _service.GetByIdAsync(id, ct);
        return show is null ? NotFound() : Ok(show);
    }

    // GET /api/tvshows/{id}/episodes?season=2
    [HttpGet("{id:int}/episodes")]
    public async Task<ActionResult<IEnumerable<EpisodeDto>>> GetEpisodesAsync([FromRoute] int id, [FromQuery] int? season, CancellationToken ct)
    {
        var show = await _service.GetByIdAsync(id, ct);
        if (show is null) return NotFound();

        var eps = season.HasValue
            ? show.Episodes.Where(e => e.Season == season.Value)
            : show.Episodes;

        return Ok(eps.OrderBy(e => e.Season).ThenBy(e => e.Number));
    }

    // GET /api/tvshows/{id}/actors
    [HttpGet("{id:int}/actors")]
    public async Task<ActionResult<IEnumerable<ActorDto>>> GetActorsAsync([FromRoute] int id, CancellationToken ct)
    {
        var show = await _service.GetByIdAsync(id, ct);
        if (show is null) return NotFound();

        return Ok(show.Cast.OrderBy(a => a.Name));
    }

    // --- Favoritos (sem auth por enquanto): usa userId=1 só para cumprir o requisito

    // GET /api/me/favorites
    // Nota: "~" faz a rota ser absoluta (ignora "api/tvshows")
    [HttpGet("~/api/me/favorites")]
    public async Task<ActionResult<IEnumerable<FavoriteShowDto>>> GetMyFavorites(CancellationToken ct)
    {
        const int userId = 1;
        var items = await _service.GetFavoritesAsync(userId, ct);
        return Ok(items);
    }

    // POST /api/me/favorites/{showId}
    [HttpPost("~/api/me/favorites/{showId:int}")]
    public async Task<ActionResult> AddFavoriteAsync([FromRoute] int showId, CancellationToken ct)
    {
        const int userId = 1;
        var ok = await _service.ToggleFavoriteAsync(userId, showId, add: true, ct);
        return ok ? NoContent() : Conflict(); // já existia
    }

    // DELETE /api/me/favorites/{showId}
    [HttpDelete("~/api/me/favorites/{showId:int}")]
    public async Task<ActionResult> RemoveFavoriteAsync([FromRoute] int showId, CancellationToken ct)
    {
        const int userId = 1;
        var ok = await _service.ToggleFavoriteAsync(userId, showId, add: false, ct);
        return ok ? NoContent() : NotFound(); // não existia
    }
}
