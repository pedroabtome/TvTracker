TV Show Tracker API (.NET 8 + PostgreSQL)

A small, SOLID, RESTful API to browse TV shows (Breaking Bad, Better Call Saul, True Detective and Fargo), all their episodes and the corresponding ratings in the IMDB site, featured actors, and to add/remove favorites.
It ships with a clean architecture, PostgreSQL via EF Core, Swagger, pagination/sorting/filtering, and a background sync worker.



This README explains what the API does, how it’s built, and how to run/evaluate it.


   - Features (challenge mapping)

List all TV shows from the database with pagination, filtering and sorting.

Show details for a TV show, including episodes (with release dates, runtime, IMDb rating) and featured actors.

Filter by genre, type, status, and free-text search.

Add/Remove favorite TV shows (for a demo user).

SQL database (PostgreSQL) with EF Core and migrations.

Background worker that can pull data from an external source and upsert it (Infrastructure/Sync).

OpenAPI/Swagger documentation.

Unit-test project scaffold (extend as needed).


   - Architecture

TvTracker/
 ├─ TvTracker.Api               → ASP.NET Core Web API (endpoints, DI, Swagger)
 ├─ TvTracker.Application       → Use cases, DTOs, validation, mapping
 ├─ TvTracker.Domain            → Entities (TvShow, Episode, Actor, Genre, Favorite, etc.)
 ├─ TvTracker.Infrastructure    → EF Core (DbContext, Migrations, Seed), Sync worker
 └─ TvTracker.Tests             → Test project (xUnit scaffold)


Clean layering: API → Application → Domain; Infrastructure provides implementations.

EF Core + Npgsql (PostgreSQL).

AutoMapper for mapping Entities ↔ DTOs.

FluentValidation for input validation.

System.Linq.Dynamic.Core for dynamic Sort.

Swashbuckle for Swagger UI.


   - Data model (high level)

TvShow: Id, Name, Type, Status, Network, Premiered, LastUpdated, Genres, Episodes, Cast

Episode: Id, TvShowId, Season, Number, Name, AirDate, Runtime, ImdbRating, Summary?, ExternalId?

Actor + CastMember (join with CharacterName)

Genre + TvShowGenres (many-to-many)

Favorite: composite key (UserId, TvShowId)

User: for the demo we seed a single User Id=1 so favorites work out-of-the-box.


   - Seeding

On startup the app:

Runs migrations.

Ensures User Id=1 exists (demo user for favorites).

Seeds a curated dataset if the DB is empty:

Breaking Bad (S1–S5, episodes with air dates, runtimes, IMDb ratings)

True Detective (S1–S4, episode data)

Better Call Saul (S1–S6, episode data)

Fargo (S1–S5, episode data)

If you already populated the DB and want to re-seed, see Re-seeding below.


   - Background Sync

A hosted service (see TvTracker.Infrastructure/Sync/EpisodeSyncWorker.cs) illustrates how to pull data from an external API and upsert it.
Extend it to call your preferred source (e.g., Episodate/TVMaze) and map into our entities.


   - Quick start

Prerequisites

.NET 8 SDK

PostgreSQL reachable at the connection string in appsettings.json:

"ConnectionStrings": {
  "Default": "Host=localhost;Port=5432;Database=tvtracker;Username=postgres;Password=postgres"
}


You can override via environment variable ConnectionStrings__Default.

1) Apply migrations (creates DB if missing)
dotnet ef database update \
  --project ./TvTracker.Infrastructure/TvTracker.Infrastructure.csproj \
  --startup-project ./TvTracker.Api/TvTracker.Api.csproj


On first run the app also applies migrations automatically and seeds the data if empty.

2) Run the API
dotnet run --project ./TvTracker.Api/TvTracker.Api.csproj


API base URL: printed to console (e.g., http://localhost:5237) 

Swagger: http://localhost:5237/swagger    (Adapt the URL to the corresponding port)


   - Re-seeding the database (optional)

The seeder runs only when there are no TvShows.

Option A – Drop and recreate DB

dotnet ef database drop -f \
  --project ./TvTracker.Infrastructure/TvTracker.Infrastructure.csproj \
  --startup-project ./TvTracker.Api/TvTracker.Api.csproj

dotnet ef database update \
  --project ./TvTracker.Infrastructure/TvTracker.Infrastructure.csproj \
  --startup-project ./TvTracker.Api/TvTracker.Api.csproj


Option B – Truncate data (psql)

TRUNCATE "Favorites" CASCADE;
TRUNCATE "Episodes"  CASCADE;
TRUNCATE "ShowActors" CASCADE;
TRUNCATE "Actors" CASCADE;
TRUNCATE "TvShowGenres" CASCADE;
TRUNCATE "Genres" CASCADE;
TRUNCATE "TvShows" CASCADE;


Restart the API; the seeder will run again.

   - API Reference (high level)

All endpoints are documented in Swagger.

List TV shows
GET /api/tvshows


Query params

search — case-insensitive match on show name

genre — exact genre name

type — e.g., Scripted

status — e.g., Ended, Returning Series

sort — "Name", "Premiered", "-Premiered", "LastUpdated", etc. (- = desc)

page, pageSize — pagination

Response

{
  "items": [ { "id": 1, "name": "Breaking Bad", "genres": ["Crime","Drama","Thriller"], ... } ],
  "page": 1,
  "pageSize": 20,
  "total": 123
}

TV show details
GET /api/tvshows/{id}


Returns show + genres + cast + episodes (each with Season, Number, Name, AirDate, Runtime, ImdbRating).

Episodes for a show
GET /api/tvshows/{id}/episodes?season=2

Featured actors for a show
GET /api/tvshows/{id}/actors

Favorites (demo user)

For the challenge we operate as User Id=1 (no auth for simplicity).

GET    /api/me/favorites
POST   /api/me/favorites/{showId}
DELETE /api/me/favorites/{showId}


POST returns 409 Conflict if the favorite already exists.

DELETE returns 404 Not Found if the favorite doesn’t exist.


   - Tests

TvTracker.Tests contains an initial xUnit project you can extend with unit/integration tests (e.g., for Application services and EF behavior).


   - Developer notes

Dynamic sorting via System.Linq.Dynamic.Core (prefix - for descending).

Validation with FluentValidation.

Mappings in Application (AutoMapper profiles).

Adding migrations after model changes:

dotnet ef migrations add <Name> \
  --project ./TvTracker.Infrastructure/TvTracker.Infrastructure.csproj \
  --startup-project ./TvTracker.Api/TvTracker.Api.csproj

dotnet ef database update \
  --project ./TvTracker.Infrastructure/TvTracker.Infrastructure.csproj \
  --startup-project ./TvTracker.Api/TvTracker.Api.csproj


   - Design choices (short)

Clean layering keeps controllers thin and business logic in Application.

EF Core + PostgreSQL for productivity and reliability.

DTOs + AutoMapper decouple persistence from API contracts.

Seeder provides a realistic dataset (Breaking Bad, True Detective, Better Call Saul, Fargo) with episodes and IMDb ratings.

Favorites modeled with composite key; seed User Id=1 to simplify demo.

Background worker shows how to fetch/merge external data.
   

   - Troubleshooting

Startup project doesn’t reference Microsoft.EntityFrameworkCore.Design
Ensure the API project includes:

<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.*">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>build; analyzers</IncludeAssets>
</PackageReference>


Seeder didn’t run: DB already has TvShows; use Re-seeding steps.

FK error when adding favorite: ensure User Id=1 exists (seeder creates it).

Port in use: change ASPNETCORE_URLS or update launchSettings.json.