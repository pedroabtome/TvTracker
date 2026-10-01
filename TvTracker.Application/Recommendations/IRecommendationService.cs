using TvTracker.Application.Recommendations.Dtos;

namespace TvTracker.Application.Recommendations;

public interface IRecommendationService
{
    Task<IReadOnlyList<RecommendationDto>> GetRecommendationsAsync(
        int userId,
        int limit = 10,
        CancellationToken ct = default);
}