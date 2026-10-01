# TV Tracker

Backend API for browsing TV shows, managing favourites and generating personalised recommendations.

Built with .NET 8, ASP.NET Core, Entity Framework Core and PostgreSQL.

## Features

- Browse, search, filter and sort TV shows
- View show details, episodes, cast and IMDb ratings
- Add and remove favourite shows
- Synchronise show and episode data from an external TV API
- Content-based recommendations based on user favourites
- Swagger/OpenAPI documentation
- Automated tests

## Architecture

The solution is split into four main layers:

```text
TvTracker.Api
TvTracker.Application
TvTracker.Domain
TvTracker.Infrastructure
```

`TvTracker.Tests` contains the test suite.

The API layer handles HTTP requests, Application contains contracts and DTOs, Domain contains the core entities, and Infrastructure handles persistence, external integrations and recommendation logic.

## Recommendation system

Recommendations are based on the user's favourite shows.

Show metadata such as genres, summary, network and status is converted into feature vectors using ML.NET text featurization. The vectors of the favourite shows are averaged into a user profile, and the remaining shows are ranked using cosine similarity.

The current demo uses `UserId = 1`.

## Tech stack

- C# / .NET 8
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- ML.NET
- Swagger
- xUnit
- Git

## Main endpoints

| Method | Endpoint |
| --- | --- |
| GET | `/api/tvshows` |
| GET | `/api/tvshows/{id}` |
| GET | `/api/tvshows/{id}/episodes` |
| GET | `/api/tvshows/{id}/actors` |
| GET | `/api/me/favorites` |
| POST | `/api/me/favorites/{showId}` |
| DELETE | `/api/me/favorites/{showId}` |
| GET | `/api/me/recommendations?limit=10` |

The show list supports search, filtering, sorting and pagination.

## Running locally

Requirements:

- .NET 8 SDK
- PostgreSQL

Apply the migrations:

```bash
dotnet ef database update \
  --project TvTracker.Infrastructure/TvTracker.Infrastructure.csproj \
  --startup-project TvTracker.Api/TvTracker.Api.csproj
```

Run the API:

```bash
dotnet run --project TvTracker.Api
```

Swagger is available at:

```text
http://localhost:<port>/swagger
```

## Tests

Run the test suite with:

```bash
dotnet test
```

The recommendation tests check that favourites are excluded, the requested result limit is respected and results are ordered by similarity score.

## Notes

The project currently uses a demo user instead of authentication.

The recommendation approach is content-based because the application does not yet have enough user interaction data for collaborative filtering.
