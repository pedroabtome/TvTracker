using TvTracker.Application.Common;
using TvTracker.Application.Shows.Dtos;

namespace TvTracker.Application.Shows;

public interface IShowService
{
    Task<PagedResult<TvShowListItemDto>> ListAsync(ListShowsQuery query, CancellationToken ct = default);
    Task<TvShowDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ToggleFavoriteAsync(int userId, int showId, bool add, CancellationToken ct = default);

    Task<IEnumerable<FavoriteShowDto>> GetFavoritesAsync(int userId, CancellationToken ct);
}
