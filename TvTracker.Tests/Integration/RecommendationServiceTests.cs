using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TvTracker.Domain.Entities;
using TvTracker.Infrastructure.Persistence;
using TvTracker.Infrastructure.Recommendations;

namespace TvTracker.Tests.Integration;

public class RecommendationServiceTests
{
    [Fact]
    public async Task GetRecommendationsAsync_ExcludesFavoritesAndRespectsLimit()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<TrackerDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new TrackerDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var crime = new Genre { Name = "Crime" };
        var drama = new Genre { Name = "Drama" };
        var comedy = new Genre { Name = "Comedy" };

        var favoriteShow = new TvShow
        {
            Name = "Breaking Bad",
            Summary = "A chemistry teacher enters the criminal world.",
            Network = "AMC",
            Status = "Ended",
            Genres = new List<Genre> { crime, drama }
        };

        var similarShow = new TvShow
        {
            Name = "Better Call Saul",
            Summary = "A lawyer becomes involved with criminals.",
            Network = "AMC",
            Status = "Ended",
            Genres = new List<Genre> { crime, drama }
        };

        var anotherSimilarShow = new TvShow
        {
            Name = "Narcos",
            Summary = "Crime drama about drug cartels.",
            Status = "Ended",
            Genres = new List<Genre> { crime, drama }
        };

        var differentShow = new TvShow
        {
            Name = "The Office",
            Summary = "A comedy about office employees.",
            Status = "Ended",
            Genres = new List<Genre> { comedy }
        };

        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = "test"
        };

        db.AddRange(user, favoriteShow, similarShow, anotherSimilarShow, differentShow);
        await db.SaveChangesAsync();

        db.Favorites.Add(new Favorite
        {
            UserId = user.Id,
            TvShowId = favoriteShow.Id
        });

        await db.SaveChangesAsync();

        var service = new RecommendationService(db);

        var result = await service.GetRecommendationsAsync(user.Id, limit: 2);

        result.Should().HaveCount(2);
        result.Should().NotContain(x => x.Id == favoriteShow.Id);
        result.Should().BeInDescendingOrder(x => x.Score);
    }
}