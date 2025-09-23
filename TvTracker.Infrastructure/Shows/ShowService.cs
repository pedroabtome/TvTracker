using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core; // para OrderBy(string)
using TvTracker.Application.Shows;
using TvTracker.Application.Shows.Dtos;
using TvTracker.Infrastructure.Persistence;

// === ALIAS para desambiguar PagedResult ===
using AppCommon = TvTracker.Application.Common;

namespace TvTracker.Infrastructure.Shows;

public class ShowService : IShowService
{
    private readonly TrackerDbContext _db;
    private readonly IMapper _mapper;

    public ShowService(TrackerDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<AppCommon.PagedResult<TvShowListItemDto>> ListAsync(
        ListShowsQuery q,
        CancellationToken ct = default)
    {
        var query = _db.TvShows.AsNoTracking().Include(s => s.Genres).AsQueryable();

        if (!string.IsNullOrWhiteSpace(q.Search))
            query = query.Where(s => EF.Functions.ILike(s.Name, $"%{q.Search}%"));

        if (!string.IsNullOrWhiteSpace(q.Genre))
            query = query.Where(s => s.Genres.Any(g => g.Name == q.Genre));

        if (!string.IsNullOrWhiteSpace(q.Type))
            query = query.Where(s => s.Type == q.Type);

        if (!string.IsNullOrWhiteSpace(q.Status))
            query = query.Where(s => s.Status == q.Status);

        // Ordenação dinâmica: "Name" ou "-Premiered" etc.
        var sort = q.Sort ?? "Name";
        var dyn = sort.StartsWith("-") ? sort[1..] + " desc" : sort;
        query = query.OrderBy(dyn);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .ProjectTo<TvShowListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return new AppCommon.PagedResult<TvShowListItemDto>(items, total, q.Page, q.PageSize);
    }

    public async Task<TvShowDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var show = await _db.TvShows
            .AsNoTracking()
            .Include(s => s.Genres)
            .Include(s => s.Episodes)
            .Include(s => s.Cast).ThenInclude(cm => cm.Actor)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        return show is null ? null : _mapper.Map<TvShowDetailDto>(show);
    }

    public async Task<bool> ToggleFavoriteAsync(int userId, int showId, bool add, CancellationToken ct = default)
    {
        var fav = await _db.Favorites.FindAsync(new object[] { userId, showId }, ct);

        if (add)
        {
            if (fav != null) return false;
            _db.Favorites.Add(new Domain.Entities.Favorite { UserId = userId, TvShowId = showId });
        }
        else
        {
            if (fav == null) return false;
            _db.Favorites.Remove(fav);
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<FavoriteShowDto>> GetFavoritesAsync(int userId, CancellationToken ct)
{
    return await _db.Favorites
        .AsNoTracking()
        .Where(f => f.UserId == userId)
        .Include(f => f.TvShow)
            .ThenInclude(s => s.Genres)
        .Select(f => new FavoriteShowDto
        {
            Id = f.TvShowId,
            Name = f.TvShow.Name,
            Genres = f.TvShow.Genres.Select(g => g.Name).ToList()
        })
        .ToListAsync(ct);
}
}

