using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using TvTracker.Application.Recommendations;
using TvTracker.Application.Recommendations.Dtos;
using TvTracker.Infrastructure.Persistence;

namespace TvTracker.Infrastructure.Recommendations;

public class RecommendationService : IRecommendationService
{
    private readonly TrackerDbContext _db;
    private readonly MLContext _mlContext;

    public RecommendationService(TrackerDbContext db)
    {
        _db = db;
        _mlContext = new MLContext(seed: 1);
    }

    public async Task<IReadOnlyList<RecommendationDto>> GetRecommendationsAsync(
        int userId,
        int limit = 10,
        CancellationToken ct = default)
    {
        var favoriteIds = await _db.Favorites
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .Select(f => f.TvShowId)
            .ToListAsync(ct);

        if (favoriteIds.Count == 0)
            return Array.Empty<RecommendationDto>();

        var shows = await _db.TvShows
            .AsNoTracking()
            .Include(s => s.Genres)
            .ToListAsync(ct);

        if (shows.Count == 0)
            return Array.Empty<RecommendationDto>();

        var input = shows
            .Select(show => new ShowInput
            {
                Text = BuildText(show.Summary, show.Network, show.Status,
                    show.Genres.Select(g => g.Name))
            })
            .ToList();

        var data = _mlContext.Data.LoadFromEnumerable(input);

        var pipeline = _mlContext.Transforms.Text.FeaturizeText(
            outputColumnName: "Features",
            inputColumnName: nameof(ShowInput.Text));

        var transformer = pipeline.Fit(data);
        var transformedData = transformer.Transform(data);

        var vectors = _mlContext.Data
            .CreateEnumerable<ShowVector>(transformedData, reuseRowObject: false)
            .Select(v => v.Features.DenseValues().ToArray())
            .ToList();

        var favoriteVectors = shows
            .Select((show, index) => new { show, index })
            .Where(x => favoriteIds.Contains(x.show.Id))
            .Select(x => vectors[x.index])
            .ToList();

        var userVector = AverageVectors(favoriteVectors);

        return shows
            .Select((show, index) => new
            {
                Show = show,
                Score = CosineSimilarity(userVector, vectors[index])
            })
            .Where(x => !favoriteIds.Contains(x.Show.Id))
            .OrderByDescending(x => x.Score)
            .Take(limit)
            .Select(x => new RecommendationDto
            {
                Id = x.Show.Id,
                Name = x.Show.Name,
                Score = Math.Round(x.Score, 3),
                Genres = x.Show.Genres.Select(g => g.Name).ToList()
            })
            .ToList();
    }

    private static string BuildText(
        string? summary,
        string? network,
        string? status,
        IEnumerable<string> genres)
    {
        return string.Join(" ",
            string.Join(" ", genres),
            summary ?? "",
            network ?? "",
            status ?? "");
    }

    private static float[] AverageVectors(IReadOnlyList<float[]> vectors)
    {
        var result = new float[vectors[0].Length];

        foreach (var vector in vectors)
        {
            for (var i = 0; i < vector.Length; i++)
                result[i] += vector[i];
        }

        for (var i = 0; i < result.Length; i++)
            result[i] /= vectors.Count;

        return result;
    }

    private static double CosineSimilarity(float[] first, float[] second)
    {
        double dotProduct = 0;
        double firstLength = 0;
        double secondLength = 0;

        for (var i = 0; i < first.Length; i++)
        {
            dotProduct += first[i] * second[i];
            firstLength += first[i] * first[i];
            secondLength += second[i] * second[i];
        }

        if (firstLength == 0 || secondLength == 0)
            return 0;

        return dotProduct /
               (Math.Sqrt(firstLength) * Math.Sqrt(secondLength));
    }

    private sealed class ShowInput
    {
        public string Text { get; set; } = "";
    }

    private sealed class ShowVector
    {
        public VBuffer<float> Features { get; set; }
    }
}