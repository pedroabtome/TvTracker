using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TvTracker.Domain.Entities;

namespace TvTracker.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task MigrateAndSeedAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TrackerDbContext>();

        await db.Database.MigrateAsync(ct);

         // Garantir que existe um utilizador com Id = 1 (necessário para /api/me/favorites)
        // Nota: o Email é NOT NULL na tua tabela, por isso preenchemos.
        if (!await db.Users.AnyAsync(u => u.Id == 1, ct))
        {
            db.Users.Add(new User
            {
                Id = 1,
                Email = "demo@example.com"
                // Se tiveres mais campos NOT NULL na entidade User, preenche aqui também.
            });

            await db.SaveChangesAsync(ct);
        }

        // se já existem shows, não volta a semear
        if (await db.TvShows.AnyAsync(ct)) return;

        // Genres
        var crime    = new Genre { Name = "Crime" };
        var drama    = new Genre { Name = "Drama" };
        var thriller = new Genre { Name = "Thriller" };

        // 1) Breaking Bad
        var breakingBad = new TvShow
        {
            Name        = "Breaking Bad",
            Type        = "Scripted",
            Status      = "Ended",
            Network     = "AMC",
            Premiered   = new DateOnly(2008, 1, 20),
            LastUpdated = DateTime.UtcNow,
            Genres      = { crime, drama, thriller }
        };

        // ------- Episódios – Breaking Bad (S1–S5) -------
// helper para tornar a lista mais legível
static Episode Ep(TvShow show, int s, int n, string name, int y, int m, int d, int runtime, decimal rating) =>
    new Episode
    {
        TvShow     = show,
        Season     = s,
        Number     = n,
        Name       = name,
        AirDate    = new DateOnly(y, m, d),
        Runtime    = runtime,
        ImdbRating = rating
    };

// S1 (7 eps)
var bbS1 = new[]
{
    Ep(breakingBad, 1, 1, "Pilot",                         2008, 1, 20, 58, 9.0m),
    Ep(breakingBad, 1, 2, "Cat's in the Bag...",           2008, 1, 27, 48, 8.6m),
    Ep(breakingBad, 1, 3, "...And the Bag's in the River", 2008, 2, 10, 48, 8.7m),
    Ep(breakingBad, 1, 4, "Cancer Man",                    2008, 2, 17, 48, 8.2m),
    Ep(breakingBad, 1, 5, "Gray Matter",                   2008, 2, 24, 53, 8.3m),
    Ep(breakingBad, 1, 6, "Crazy Handful of Nothin'",      2008, 3, 2,  48, 8.7m),
    Ep(breakingBad, 1, 7, "A No-Rough-Stuff-Type Deal",    2008, 3, 9,  48, 8.8m),
};

// S2 (13 eps)
var bbS2 = new[]
{
    Ep(breakingBad, 2, 1,  "Seven Thirty-Seven", 2009, 3,  8, 48, 8.6m),
    Ep(breakingBad, 2, 2,  "Grilled",            2009, 3, 15, 48, 9.3m),
    Ep(breakingBad, 2, 3,  "Bit by a Dead Bee",  2009, 3, 22, 48, 8.3m),
    Ep(breakingBad, 2, 4,  "Down",               2009, 3, 29, 48, 8.2m),
    Ep(breakingBad, 2, 5,  "Breakage",           2009, 4,  5, 48, 8.2m),
    Ep(breakingBad, 2, 6,  "Peekaboo",           2009, 4, 12, 48, 8.8m),
    Ep(breakingBad, 2, 7,  "Negro y Azul",       2009, 4, 19, 48, 8.6m),
    Ep(breakingBad, 2, 8,  "Better Call Saul",   2009, 4, 26, 48, 9.2m),
    Ep(breakingBad, 2, 9,  "4 Days Out",         2009, 5,  3, 48, 9.2m),
    Ep(breakingBad, 2, 10, "Over",               2009, 5, 10, 48, 8.4m),
    Ep(breakingBad, 2, 11, "Mandala",            2009, 5, 17, 48, 8.9m),
    Ep(breakingBad, 2, 12, "Phoenix",            2009, 5, 24, 48, 9.3m),
    Ep(breakingBad, 2, 13, "ABQ",                2009, 5, 31, 48, 9.2m),
};

// S3 (13 eps)
var bbS3 = new[]
{
    Ep(breakingBad, 3, 1,  "No Más",             2010, 3, 21, 48, 8.5m),
    Ep(breakingBad, 3, 2,  "Caballo sin Nombre", 2010, 3, 28, 48, 8.6m),
    Ep(breakingBad, 3, 3,  "I.F.T.",             2010, 4,  4, 48, 8.4m),
    Ep(breakingBad, 3, 4,  "Green Light",        2010, 4, 11, 48, 8.1m),
    Ep(breakingBad, 3, 5,  "Más",                2010, 4, 18, 48, 8.5m),
    Ep(breakingBad, 3, 6,  "Sunset",             2010, 4, 25, 48, 9.3m),
    Ep(breakingBad, 3, 7,  "One Minute",         2010, 5,  2, 48, 9.6m),
    Ep(breakingBad, 3, 8,  "I See You",          2010, 5,  9, 48, 8.7m),
    Ep(breakingBad, 3, 9,  "Kafkaesque",         2010, 5, 16, 48, 8.4m),
    Ep(breakingBad, 3, 10, "Fly",                2010, 5, 23, 48, 8.0m),
    Ep(breakingBad, 3, 11, "Abiquiu",            2010, 5, 30, 48, 8.4m),
    Ep(breakingBad, 3, 12, "Half Measures",      2010, 6,  6, 48, 9.5m),
    Ep(breakingBad, 3, 13, "Full Measure",       2010, 6, 13, 48, 9.7m),
};

// S4 (13 eps)
var bbS4 = new[]
{
    Ep(breakingBad, 4, 1,  "Box Cutter",     2011, 7, 17, 48, 9.2m),
    Ep(breakingBad, 4, 2,  "Thirty-Eight Snub", 2011, 7, 24, 48, 8.2m),
    Ep(breakingBad, 4, 3,  "Open House",     2011, 7, 31, 48, 8.0m),
    Ep(breakingBad, 4, 4,  "Bullet Points",  2011, 8,  7, 48, 8.5m),
    Ep(breakingBad, 4, 5,  "Shotgun",        2011, 8, 14, 48, 8.6m),
    Ep(breakingBad, 4, 6,  "Cornered",       2011, 8, 21, 48, 8.4m),
    Ep(breakingBad, 4, 7,  "Problem Dog",    2011, 8, 28, 48, 8.8m),
    Ep(breakingBad, 4, 8,  "Hermanos",       2011, 9,  4, 48, 9.3m),
    Ep(breakingBad, 4, 9,  "Bug",            2011, 9, 11, 48, 8.9m),
    Ep(breakingBad, 4, 10, "Salud",          2011, 9, 18, 48, 9.6m),
    Ep(breakingBad, 4, 11, "Crawl Space",    2011, 9, 25, 47, 9.7m),
    Ep(breakingBad, 4, 12, "End Times",      2011, 10, 2, 48, 9.5m),
    Ep(breakingBad, 4, 13, "Face Off",       2011, 10, 9, 48, 9.9m),
};

// S5 (16 eps)
var bbS5 = new[]
{
    Ep(breakingBad, 5, 1,  "Live Free or Die",   2012, 7, 15, 48, 9.2m),
    Ep(breakingBad, 5, 2,  "Madrigal",           2012, 7, 22, 48, 8.8m),
    Ep(breakingBad, 5, 3,  "Hazard Pay",         2012, 7, 29, 48, 8.8m),
    Ep(breakingBad, 5, 4,  "Fifty-One",          2012, 8,  5, 48, 8.9m),
    Ep(breakingBad, 5, 5,  "Dead Freight",       2012, 8, 12, 48, 9.7m),
    Ep(breakingBad, 5, 6,  "Buyout",             2012, 8, 19, 48, 9.0m),
    Ep(breakingBad, 5, 7,  "Say My Name",        2012, 8, 26, 48, 9.6m),
    Ep(breakingBad, 5, 8,  "Gliding Over All",   2012, 9,  2, 48, 9.6m),

    Ep(breakingBad, 5, 9,  "Blood Money",        2013, 8, 11, 48, 9.4m),
    Ep(breakingBad, 5, 10, "Buried",             2013, 8, 18, 48, 8.7m),
    Ep(breakingBad, 5, 11, "Confessions",        2013, 8, 25, 48, 9.6m),
    Ep(breakingBad, 5, 12, "Rabid Dog",          2013, 9,  1, 48, 9.2m),
    Ep(breakingBad, 5, 13, "To'hajiilee",        2013, 9,  8, 48, 9.8m),
    Ep(breakingBad, 5, 14, "Ozymandias",         2013, 9, 15, 49, 10.0m),
    Ep(breakingBad, 5, 15, "Granite State",      2013, 9, 22, 55, 9.7m),
    Ep(breakingBad, 5, 16, "Felina",             2013, 9, 29, 55, 9.9m),
};

// adiciona ao contexto
db.Episodes.AddRange(bbS1);
db.Episodes.AddRange(bbS2);
db.Episodes.AddRange(bbS3);
db.Episodes.AddRange(bbS4);
db.Episodes.AddRange(bbS5);


        // Atores principais
        var bryanCranston = new Actor { Name = "Bryan Cranston" };
        var aaronPaul     = new Actor { Name = "Aaron Paul" };
        var annaGunn      = new Actor { Name = "Anna Gunn" };
        db.Actors.AddRange(bryanCranston, aaronPaul, annaGunn);

        db.CastMembers.AddRange(
            new CastMember { TvShow = breakingBad, Actor = bryanCranston, CharacterName = "Walter White" },
            new CastMember { TvShow = breakingBad, Actor = aaronPaul,     CharacterName = "Jesse Pinkman" },
            new CastMember { TvShow = breakingBad, Actor = annaGunn,      CharacterName = "Skyler White" }
        );

        // 2) True Detective
        var trueDetective = new TvShow
        {
            Name        = "True Detective",
            Type        = "Scripted",
            Status      = "Returning Series",
            Network     = "HBO",
            Premiered   = new DateOnly(2014, 1, 12),
            LastUpdated = DateTime.UtcNow,
            Genres      = { crime, drama, thriller }
        };

       // ---------------------------
// TRUE DETECTIVE — episódios
// ---------------------------
db.Episodes.AddRange(
    // S1 (2014)
    new Episode { TvShow = trueDetective, Season = 1, Number = 1, Name = "The Long Bright Dark",        AirDate = new DateOnly(2014, 1, 12), ImdbRating = 8.9m },
    new Episode { TvShow = trueDetective, Season = 1, Number = 2, Name = "Seeing Things",               AirDate = new DateOnly(2014, 1, 19), ImdbRating = 8.8m },
    new Episode { TvShow = trueDetective, Season = 1, Number = 3, Name = "The Locked Room",             AirDate = new DateOnly(2014, 1, 26), ImdbRating = 9.1m },
    new Episode { TvShow = trueDetective, Season = 1, Number = 4, Name = "Who Goes There",              AirDate = new DateOnly(2014, 2,  9), ImdbRating = 9.6m },
    new Episode { TvShow = trueDetective, Season = 1, Number = 5, Name = "The Secret Fate of All Life", AirDate = new DateOnly(2014, 2, 16), ImdbRating = 9.5m },
    new Episode { TvShow = trueDetective, Season = 1, Number = 6, Name = "Haunted Houses",              AirDate = new DateOnly(2014, 2, 23), ImdbRating = 9.2m },
    new Episode { TvShow = trueDetective, Season = 1, Number = 7, Name = "After You've Gone",           AirDate = new DateOnly(2014, 3,  2), ImdbRating = 9.2m },
    new Episode { TvShow = trueDetective, Season = 1, Number = 8, Name = "Form and Void",               AirDate = new DateOnly(2014, 3,  9), ImdbRating = 9.6m },

    // S2 (2015)
    new Episode { TvShow = trueDetective, Season = 2, Number = 1, Name = "The Western Book of the Dead", AirDate = new DateOnly(2015, 6, 21), ImdbRating = 7.5m },
    new Episode { TvShow = trueDetective, Season = 2, Number = 2, Name = "Night Finds You",              AirDate = new DateOnly(2015, 6, 28), ImdbRating = 7.6m },
    new Episode { TvShow = trueDetective, Season = 2, Number = 3, Name = "Maybe Tomorrow",               AirDate = new DateOnly(2015, 7,  5), ImdbRating = 7.3m },
    new Episode { TvShow = trueDetective, Season = 2, Number = 4, Name = "Down Will Come",               AirDate = new DateOnly(2015, 7, 12), ImdbRating = 8.1m },
    new Episode { TvShow = trueDetective, Season = 2, Number = 5, Name = "Other Lives",                  AirDate = new DateOnly(2015, 7, 19), ImdbRating = 7.6m },
    new Episode { TvShow = trueDetective, Season = 2, Number = 6, Name = "Church in Ruins",              AirDate = new DateOnly(2015, 7, 26), ImdbRating = 8.2m },
    new Episode { TvShow = trueDetective, Season = 2, Number = 7, Name = "Black Maps and Motel Rooms",   AirDate = new DateOnly(2015, 8,  2), ImdbRating = 8.5m },
    new Episode { TvShow = trueDetective, Season = 2, Number = 8, Name = "Omega Station",                AirDate = new DateOnly(2015, 8,  9), ImdbRating = 8.0m },

    // S3 (2019)
    new Episode { TvShow = trueDetective, Season = 3, Number = 1, Name = "The Great War and Modern Memory", AirDate = new DateOnly(2019, 1, 13), ImdbRating = 8.5m },
    new Episode { TvShow = trueDetective, Season = 3, Number = 2, Name = "Kiss Tomorrow Goodbye",           AirDate = new DateOnly(2019, 1, 13), ImdbRating = 8.0m },
    new Episode { TvShow = trueDetective, Season = 3, Number = 3, Name = "The Big Never",                    AirDate = new DateOnly(2019, 1, 20), ImdbRating = 7.7m },
    new Episode { TvShow = trueDetective, Season = 3, Number = 4, Name = "The Hour and the Day",             AirDate = new DateOnly(2019, 1, 27), ImdbRating = 8.0m },
    new Episode { TvShow = trueDetective, Season = 3, Number = 5, Name = "If You Have Ghosts",               AirDate = new DateOnly(2019, 2,  1), ImdbRating = 8.3m },
    new Episode { TvShow = trueDetective, Season = 3, Number = 6, Name = "Hunters in the Dark",              AirDate = new DateOnly(2019, 2, 10), ImdbRating = 8.3m },
    new Episode { TvShow = trueDetective, Season = 3, Number = 7, Name = "The Final Country",                AirDate = new DateOnly(2019, 2, 17), ImdbRating = 8.5m },
    new Episode { TvShow = trueDetective, Season = 3, Number = 8, Name = "Now Am Found",                     AirDate = new DateOnly(2019, 2, 24), ImdbRating = 8.0m },

    // S4 (2024) — Night Country
    new Episode { TvShow = trueDetective, Season = 4, Number = 1, Name = "Night Country: Part 1", AirDate = new DateOnly(2024, 1, 14), ImdbRating = 7.1m },
    new Episode { TvShow = trueDetective, Season = 4, Number = 2, Name = "Night Country: Part 2", AirDate = new DateOnly(2024, 1, 21), ImdbRating = 6.8m },
    new Episode { TvShow = trueDetective, Season = 4, Number = 3, Name = "Night Country: Part 3", AirDate = new DateOnly(2024, 1, 28), ImdbRating = 6.3m },
    new Episode { TvShow = trueDetective, Season = 4, Number = 4, Name = "Night Country: Part 4", AirDate = new DateOnly(2024, 2,  4), ImdbRating = 6.2m },
    new Episode { TvShow = trueDetective, Season = 4, Number = 5, Name = "Night Country: Part 5", AirDate = new DateOnly(2024, 2,  9), ImdbRating = 6.8m },
    new Episode { TvShow = trueDetective, Season = 4, Number = 6, Name = "Night Country: Part 6", AirDate = new DateOnly(2024, 2, 18), ImdbRating = 5.4m }
);


        var mcConaughey      = new Actor { Name = "Matthew McConaughey" };
        var woodyHarrelson   = new Actor { Name = "Woody Harrelson" };
        var michelleMonaghan = new Actor { Name = "Michelle Monaghan" };
        db.Actors.AddRange(mcConaughey, woodyHarrelson, michelleMonaghan);

        db.CastMembers.AddRange(
            new CastMember { TvShow = trueDetective, Actor = mcConaughey,      CharacterName = "Rust Cohle" },
            new CastMember { TvShow = trueDetective, Actor = woodyHarrelson,   CharacterName = "Marty Hart" },
            new CastMember { TvShow = trueDetective, Actor = michelleMonaghan, CharacterName = "Maggie Hart" }
        );

        // 3) Better Call Saul
        var betterCallSaul = new TvShow
        {
            Name        = "Better Call Saul",
            Type        = "Scripted",
            Status      = "Ended",
            Network     = "AMC",
            Premiered   = new DateOnly(2015, 2, 8),
            LastUpdated = DateTime.UtcNow,
            Genres      = { crime, drama }
        };

       // Episódios — Better Call Saul (todas as temporadas)
var bcsEpisodes = new[]
{
    // --- Season 1 (2015)
    new Episode { TvShow = betterCallSaul, Season = 1, Number = 1,  Name = "Uno",                    AirDate = new DateOnly(2015, 2,  8), Runtime = null, ImdbRating = 8.4m },
    new Episode { TvShow = betterCallSaul, Season = 1, Number = 2,  Name = "Mijo",                   AirDate = new DateOnly(2015, 2,  9), Runtime = null, ImdbRating = 8.9m },
    new Episode { TvShow = betterCallSaul, Season = 1, Number = 3,  Name = "Nacho",                  AirDate = new DateOnly(2015, 2, 16), Runtime = null, ImdbRating = 8.6m },
    new Episode { TvShow = betterCallSaul, Season = 1, Number = 4,  Name = "Hero",                   AirDate = new DateOnly(2015, 2, 23), Runtime = null, ImdbRating = 8.3m },
    new Episode { TvShow = betterCallSaul, Season = 1, Number = 5,  Name = "Alpine Shepherd Boy",    AirDate = new DateOnly(2015, 3,  2), Runtime = null, ImdbRating = 8.0m },
    new Episode { TvShow = betterCallSaul, Season = 1, Number = 6,  Name = "Five-O",                 AirDate = new DateOnly(2015, 3,  9), Runtime = null, ImdbRating = 9.4m },
    new Episode { TvShow = betterCallSaul, Season = 1, Number = 7,  Name = "Bingo",                  AirDate = new DateOnly(2015, 3, 16), Runtime = null, ImdbRating = 8.6m },
    new Episode { TvShow = betterCallSaul, Season = 1, Number = 8,  Name = "RICO",                   AirDate = new DateOnly(2015, 3, 23), Runtime = null, ImdbRating = 8.6m },
    new Episode { TvShow = betterCallSaul, Season = 1, Number = 9,  Name = "Pimento",                AirDate = new DateOnly(2015, 3, 30), Runtime = null, ImdbRating = 9.4m },
    new Episode { TvShow = betterCallSaul, Season = 1, Number = 10, Name = "Marco",                  AirDate = new DateOnly(2015, 4,  6), Runtime = null, ImdbRating = 8.6m },

    // --- Season 2 (2016)
    new Episode { TvShow = betterCallSaul, Season = 2, Number = 1,  Name = "Switch",                 AirDate = new DateOnly(2016, 2, 15), Runtime = null, ImdbRating = 8.2m },
    new Episode { TvShow = betterCallSaul, Season = 2, Number = 2,  Name = "Cobbler",                AirDate = new DateOnly(2016, 2, 22), Runtime = null, ImdbRating = 8.4m },
    new Episode { TvShow = betterCallSaul, Season = 2, Number = 3,  Name = "Amarillo",               AirDate = new DateOnly(2016, 2, 29), Runtime = null, ImdbRating = 8.2m },
    new Episode { TvShow = betterCallSaul, Season = 2, Number = 4,  Name = "Gloves Off",             AirDate = new DateOnly(2016, 3,  7), Runtime = null, ImdbRating = 8.9m },
    new Episode { TvShow = betterCallSaul, Season = 2, Number = 5,  Name = "Rebecca",                AirDate = new DateOnly(2016, 3, 14), Runtime = null, ImdbRating = 8.0m },
    new Episode { TvShow = betterCallSaul, Season = 2, Number = 6,  Name = "Bali Ha'i",              AirDate = new DateOnly(2016, 3, 21), Runtime = null, ImdbRating = 8.4m },
    new Episode { TvShow = betterCallSaul, Season = 2, Number = 7,  Name = "Inflatable",             AirDate = new DateOnly(2016, 3, 28), Runtime = null, ImdbRating = 8.3m },
    new Episode { TvShow = betterCallSaul, Season = 2, Number = 8,  Name = "Fifi",                   AirDate = new DateOnly(2016, 4,  4), Runtime = null, ImdbRating = 8.3m },
    new Episode { TvShow = betterCallSaul, Season = 2, Number = 9,  Name = "Nailed",                 AirDate = new DateOnly(2016, 4, 11), Runtime = null, ImdbRating = 9.3m },
    new Episode { TvShow = betterCallSaul, Season = 2, Number = 10, Name = "Klick",                  AirDate = new DateOnly(2016, 4, 18), Runtime = null, ImdbRating = 9.0m },

    // --- Season 3 (2017)
    new Episode { TvShow = betterCallSaul, Season = 3, Number = 1,  Name = "Mabel",                  AirDate = new DateOnly(2017, 4, 10), Runtime = null, ImdbRating = 8.1m },
    new Episode { TvShow = betterCallSaul, Season = 3, Number = 2,  Name = "Witness",                AirDate = new DateOnly(2017, 4, 17), Runtime = null, ImdbRating = 9.0m },
    new Episode { TvShow = betterCallSaul, Season = 3, Number = 3,  Name = "Sunk Costs",             AirDate = new DateOnly(2017, 4, 24), Runtime = null, ImdbRating = 8.4m },
    new Episode { TvShow = betterCallSaul, Season = 3, Number = 4,  Name = "Sabrosito",              AirDate = new DateOnly(2017, 5,  1), Runtime = null, ImdbRating = 8.9m },
    new Episode { TvShow = betterCallSaul, Season = 3, Number = 5,  Name = "Chicanery",              AirDate = new DateOnly(2017, 5,  8), Runtime = null, ImdbRating = 9.7m },
    new Episode { TvShow = betterCallSaul, Season = 3, Number = 6,  Name = "Off Brand",              AirDate = new DateOnly(2017, 5, 15), Runtime = null, ImdbRating = 8.4m },
    new Episode { TvShow = betterCallSaul, Season = 3, Number = 7,  Name = "Expenses",               AirDate = new DateOnly(2017, 5, 22), Runtime = null, ImdbRating = 8.2m },
    new Episode { TvShow = betterCallSaul, Season = 3, Number = 8,  Name = "Slip",                   AirDate = new DateOnly(2017, 6,  5), Runtime = null, ImdbRating = 8.5m },
    new Episode { TvShow = betterCallSaul, Season = 3, Number = 9,  Name = "Fall",                   AirDate = new DateOnly(2017, 6, 12), Runtime = null, ImdbRating = 8.8m },
    new Episode { TvShow = betterCallSaul, Season = 3, Number = 10, Name = "Lantern",                AirDate = new DateOnly(2017, 6, 19), Runtime = null, ImdbRating = 9.2m },

    // --- Season 4 (2018)
    new Episode { TvShow = betterCallSaul, Season = 4, Number = 1,  Name = "Smoke",                  AirDate = new DateOnly(2018, 8,  6), Runtime = null, ImdbRating = 8.2m },
    new Episode { TvShow = betterCallSaul, Season = 4, Number = 2,  Name = "Breathe",                AirDate = new DateOnly(2018, 8, 13), Runtime = null, ImdbRating = 8.8m },
    new Episode { TvShow = betterCallSaul, Season = 4, Number = 3,  Name = "Something Beautiful",    AirDate = new DateOnly(2018, 8, 20), Runtime = null, ImdbRating = 8.4m },
    new Episode { TvShow = betterCallSaul, Season = 4, Number = 4,  Name = "Talk",                   AirDate = new DateOnly(2018, 8, 27), Runtime = null, ImdbRating = 8.2m },
    new Episode { TvShow = betterCallSaul, Season = 4, Number = 5,  Name = "Quite a Ride",           AirDate = new DateOnly(2018, 9,  3), Runtime = null, ImdbRating = 8.5m },
    new Episode { TvShow = betterCallSaul, Season = 4, Number = 6,  Name = "Piñata",                 AirDate = new DateOnly(2018, 9, 10), Runtime = null, ImdbRating = 8.5m },
    new Episode { TvShow = betterCallSaul, Season = 4, Number = 7,  Name = "Something Stupid",       AirDate = new DateOnly(2018, 9, 17), Runtime = null, ImdbRating = 8.3m },
    new Episode { TvShow = betterCallSaul, Season = 4, Number = 8,  Name = "Coushatta",              AirDate = new DateOnly(2018, 9, 24), Runtime = null, ImdbRating = 8.8m },
    new Episode { TvShow = betterCallSaul, Season = 4, Number = 9,  Name = "Wiedersehen",            AirDate = new DateOnly(2018, 10, 1), Runtime = null, ImdbRating = 9.1m },
    new Episode { TvShow = betterCallSaul, Season = 4, Number = 10, Name = "Winner",                 AirDate = new DateOnly(2018, 10, 8), Runtime = null, ImdbRating = 9.6m },

    // --- Season 5 (2020)
    new Episode { TvShow = betterCallSaul, Season = 5, Number = 1,  Name = "Magic Man",              AirDate = new DateOnly(2020, 2, 23), Runtime = null, ImdbRating = 8.7m },
    new Episode { TvShow = betterCallSaul, Season = 5, Number = 2,  Name = "50% Off",                AirDate = new DateOnly(2020, 2, 24), Runtime = null, ImdbRating = 8.7m },
    new Episode { TvShow = betterCallSaul, Season = 5, Number = 3,  Name = "The Guy for This",       AirDate = new DateOnly(2020, 3,  2), Runtime = null, ImdbRating = 9.0m },
    new Episode { TvShow = betterCallSaul, Season = 5, Number = 4,  Name = "Namaste",                AirDate = new DateOnly(2020, 3,  9), Runtime = null, ImdbRating = 8.5m },
    new Episode { TvShow = betterCallSaul, Season = 5, Number = 5,  Name = "Dedicado a Max",         AirDate = new DateOnly(2020, 3, 16), Runtime = null, ImdbRating = 8.5m },
    new Episode { TvShow = betterCallSaul, Season = 5, Number = 6,  Name = "Wexler v. Goodman",      AirDate = new DateOnly(2020, 3, 23), Runtime = null, ImdbRating = 9.3m },
    new Episode { TvShow = betterCallSaul, Season = 5, Number = 7,  Name = "JMM",                    AirDate = new DateOnly(2020, 3, 30), Runtime = null, ImdbRating = 9.1m },
    new Episode { TvShow = betterCallSaul, Season = 5, Number = 8,  Name = "Bagman",                 AirDate = new DateOnly(2020, 4,  6), Runtime = null, ImdbRating = 9.7m },
    new Episode { TvShow = betterCallSaul, Season = 5, Number = 9,  Name = "Bad Choice Road",        AirDate = new DateOnly(2020, 4, 13), Runtime = null, ImdbRating = 9.7m },
    new Episode { TvShow = betterCallSaul, Season = 5, Number = 10, Name = "Something Unforgivable", AirDate = new DateOnly(2020, 4, 20), Runtime = null, ImdbRating = 9.4m },

    // --- Season 6 (2022)
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 1,  Name = "Wine and Roses",         AirDate = new DateOnly(2022, 4, 18), Runtime = null, ImdbRating = 8.8m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 2,  Name = "Carrot and Stick",       AirDate = new DateOnly(2022, 4, 18), Runtime = null, ImdbRating = 9.1m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 3,  Name = "Rock and Hard Place",    AirDate = new DateOnly(2022, 4, 25), Runtime = null, ImdbRating = 9.7m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 4,  Name = "Hit and Run",            AirDate = new DateOnly(2022, 5,  2), Runtime = null, ImdbRating = 8.6m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 5,  Name = "Black and Blue",         AirDate = new DateOnly(2022, 5,  9), Runtime = null, ImdbRating = 8.1m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 6,  Name = "Axe and Grind",          AirDate = new DateOnly(2022, 5, 16), Runtime = null, ImdbRating = 8.2m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 7,  Name = "Plan and Execution",     AirDate = new DateOnly(2022, 5, 23), Runtime = null, ImdbRating = 9.9m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 8,  Name = "Point and Shoot",        AirDate = new DateOnly(2022, 7, 11), Runtime = null, ImdbRating = 9.8m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 9,  Name = "Fun and Games",          AirDate = new DateOnly(2022, 7, 18), Runtime = null, ImdbRating = 9.4m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 10, Name = "Nippy",                  AirDate = new DateOnly(2022, 7, 25), Runtime = null, ImdbRating = 8.5m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 11, Name = "Breaking Bad",           AirDate = new DateOnly(2022, 8,  1), Runtime = null, ImdbRating = 9.0m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 12, Name = "Waterworks",             AirDate = new DateOnly(2022, 8,  8), Runtime = null, ImdbRating = 9.4m },
    new Episode { TvShow = betterCallSaul, Season = 6, Number = 13, Name = "Saul Gone",              AirDate = new DateOnly(2022, 8, 15), Runtime = null, ImdbRating = 9.8m },
};

// regista os episódios
db.Episodes.AddRange(bcsEpisodes);


        var bobOdenkirk   = new Actor { Name = "Bob Odenkirk" };
        var rheaSeehorn   = new Actor { Name = "Rhea Seehorn" };
        var jonathanBanks = new Actor { Name = "Jonathan Banks" };
        db.Actors.AddRange(bobOdenkirk, rheaSeehorn, jonathanBanks);

        db.CastMembers.AddRange(
            new CastMember { TvShow = betterCallSaul, Actor = bobOdenkirk,   CharacterName = "Jimmy McGill" },
            new CastMember { TvShow = betterCallSaul, Actor = rheaSeehorn,   CharacterName = "Kim Wexler" },
            new CastMember { TvShow = betterCallSaul, Actor = jonathanBanks, CharacterName = "Mike Ehrmantraut" }
        );

        // 4) Fargo
        var fargo = new TvShow
        {
            Name        = "Fargo",
            Type        = "Scripted",
            Status      = "Returning Series",
            Network     = "FX",
            Premiered   = new DateOnly(2014, 4, 15),
            LastUpdated = DateTime.UtcNow,
            Genres      = { crime, drama, thriller }
        };

        // -----------------------------
// Fargo — episódios (S1 a S5)
// -----------------------------

// S1 (2014)
var fargoS1E1  = new Episode { TvShow = fargo, Season = 1, Number = 1,  Name = "The Crocodile's Dilemma", AirDate = new DateOnly(2014, 4, 15), ImdbRating = 9.3m };
var fargoS1E2  = new Episode { TvShow = fargo, Season = 1, Number = 2,  Name = "The Rooster Prince",      AirDate = new DateOnly(2014, 4, 22), ImdbRating = 8.3m };
var fargoS1E3  = new Episode { TvShow = fargo, Season = 1, Number = 3,  Name = "A Muddy Road",            AirDate = new DateOnly(2014, 4, 29), ImdbRating = 8.5m };
var fargoS1E4  = new Episode { TvShow = fargo, Season = 1, Number = 4,  Name = "Eating the Blame",        AirDate = new DateOnly(2014, 5,  6), ImdbRating = 8.9m };
var fargoS1E5  = new Episode { TvShow = fargo, Season = 1, Number = 5,  Name = "The Six Ungraspables",    AirDate = new DateOnly(2014, 5, 13), ImdbRating = 8.4m };
var fargoS1E6  = new Episode { TvShow = fargo, Season = 1, Number = 6,  Name = "Buridan's Ass",           AirDate = new DateOnly(2014, 5, 20), ImdbRating = 9.2m };
var fargoS1E7  = new Episode { TvShow = fargo, Season = 1, Number = 7,  Name = "Who Shaves the Barber?",  AirDate = new DateOnly(2014, 5, 27), ImdbRating = 8.9m };
var fargoS1E8  = new Episode { TvShow = fargo, Season = 1, Number = 8,  Name = "The Heap",                AirDate = new DateOnly(2014, 6,  3), ImdbRating = 8.5m };
var fargoS1E9  = new Episode { TvShow = fargo, Season = 1, Number = 9,  Name = "A Fox, a Rabbit, and a Cabbage", AirDate = new DateOnly(2014, 6, 10), ImdbRating = 9.3m };
var fargoS1E10 = new Episode { TvShow = fargo, Season = 1, Number = 10, Name = "Morton's Fork",           AirDate = new DateOnly(2014, 6, 17), ImdbRating = 9.3m };

// S2 (2015)
var fargoS2E1  = new Episode { TvShow = fargo, Season = 2, Number = 1,  Name = "Waiting for Dutch",       AirDate = new DateOnly(2015,10,12), ImdbRating = 8.7m };
var fargoS2E2  = new Episode { TvShow = fargo, Season = 2, Number = 2,  Name = "Before the Law",          AirDate = new DateOnly(2015,10,19), ImdbRating = 8.4m };
var fargoS2E3  = new Episode { TvShow = fargo, Season = 2, Number = 3,  Name = "The Myth of Sisyphus",    AirDate = new DateOnly(2015,10,26), ImdbRating = 8.5m };
var fargoS2E4  = new Episode { TvShow = fargo, Season = 2, Number = 4,  Name = "Fear and Trembling",      AirDate = new DateOnly(2015,11, 2), ImdbRating = 8.7m };
var fargoS2E5  = new Episode { TvShow = fargo, Season = 2, Number = 5,  Name = "The Gift of the Magi",    AirDate = new DateOnly(2015,11, 9), ImdbRating = 9.1m };
var fargoS2E6  = new Episode { TvShow = fargo, Season = 2, Number = 6,  Name = "Rhinoceros",              AirDate = new DateOnly(2015,11,16), ImdbRating = 9.2m };
var fargoS2E7  = new Episode { TvShow = fargo, Season = 2, Number = 7,  Name = "Did You Do This? No, You Did It!", AirDate = new DateOnly(2015,11,23), ImdbRating = 8.8m };
var fargoS2E8  = new Episode { TvShow = fargo, Season = 2, Number = 8,  Name = "Loplop",                  AirDate = new DateOnly(2015,11,30), ImdbRating = 9.4m };
var fargoS2E9  = new Episode { TvShow = fargo, Season = 2, Number = 9,  Name = "The Castle",              AirDate = new DateOnly(2015,12, 7), ImdbRating = 9.4m };
var fargoS2E10 = new Episode { TvShow = fargo, Season = 2, Number = 10, Name = "Palindrome",              AirDate = new DateOnly(2015,12,14), ImdbRating = 8.4m };

// S3 (2017)
var fargoS3E1  = new Episode { TvShow = fargo, Season = 3, Number = 1,  Name = "The Law of Vacant Places",          AirDate = new DateOnly(2017, 4,19), ImdbRating = 8.4m };
var fargoS3E2  = new Episode { TvShow = fargo, Season = 3, Number = 2,  Name = "The Principle of Restricted Choice", AirDate = new DateOnly(2017, 4,26), ImdbRating = 8.0m };
var fargoS3E3  = new Episode { TvShow = fargo, Season = 3, Number = 3,  Name = "The Law of Non-Contradiction",      AirDate = new DateOnly(2017, 5, 3), ImdbRating = 7.9m };
var fargoS3E4  = new Episode { TvShow = fargo, Season = 3, Number = 4,  Name = "The Narrow Escape Problem",         AirDate = new DateOnly(2017, 5,10), ImdbRating = 8.1m };
var fargoS3E5  = new Episode { TvShow = fargo, Season = 3, Number = 5,  Name = "The House of Special Purpose",      AirDate = new DateOnly(2017, 5,17), ImdbRating = 8.2m };
var fargoS3E6  = new Episode { TvShow = fargo, Season = 3, Number = 6,  Name = "The Lord of No Mercy",              AirDate = new DateOnly(2017, 5,24), ImdbRating = 8.6m };
var fargoS3E7  = new Episode { TvShow = fargo, Season = 3, Number = 7,  Name = "The Law of Inevitability",          AirDate = new DateOnly(2017, 5,31), ImdbRating = 8.3m };
var fargoS3E8  = new Episode { TvShow = fargo, Season = 3, Number = 8,  Name = "Who Rules the Land of Denial?",     AirDate = new DateOnly(2017, 6, 7), ImdbRating = 8.9m };
var fargoS3E9  = new Episode { TvShow = fargo, Season = 3, Number = 9,  Name = "Aporia",                              AirDate = new DateOnly(2017, 6,14), ImdbRating = 8.7m };
var fargoS3E10 = new Episode { TvShow = fargo, Season = 3, Number = 10, Name = "Somebody to Love",                   AirDate = new DateOnly(2017, 6,21), ImdbRating = 8.5m };

// S4 (2020)
var fargoS4E1  = new Episode { TvShow = fargo, Season = 4, Number = 1,  Name = "Welcome to the Alternate Economy", AirDate = new DateOnly(2020, 9,27), ImdbRating = 7.1m };
var fargoS4E2  = new Episode { TvShow = fargo, Season = 4, Number = 2,  Name = "The Land of Taking and Killing",   AirDate = new DateOnly(2020, 9,27), ImdbRating = 7.1m };
var fargoS4E3  = new Episode { TvShow = fargo, Season = 4, Number = 3,  Name = "Raddoppiare",                        AirDate = new DateOnly(2020,10, 4), ImdbRating = 7.3m };
var fargoS4E4  = new Episode { TvShow = fargo, Season = 4, Number = 4,  Name = "The Pretend War",                    AirDate = new DateOnly(2020,10,11), ImdbRating = 7.3m };
var fargoS4E5  = new Episode { TvShow = fargo, Season = 4, Number = 5,  Name = "The Birthplace of Civilization",     AirDate = new DateOnly(2020,10,18), ImdbRating = 7.3m };
var fargoS4E6  = new Episode { TvShow = fargo, Season = 4, Number = 6,  Name = "Camp Elegance",                      AirDate = new DateOnly(2020,10,25), ImdbRating = 7.4m };
var fargoS4E7  = new Episode { TvShow = fargo, Season = 4, Number = 7,  Name = "Lay Away",                           AirDate = new DateOnly(2020,11, 1), ImdbRating = 7.2m };
var fargoS4E8  = new Episode { TvShow = fargo, Season = 4, Number = 8,  Name = "The Nadir",                          AirDate = new DateOnly(2020,11, 8), ImdbRating = 7.8m };
var fargoS4E9  = new Episode { TvShow = fargo, Season = 4, Number = 9,  Name = "East/West",                          AirDate = new DateOnly(2020,11,15), ImdbRating = 7.7m };
var fargoS4E10 = new Episode { TvShow = fargo, Season = 4, Number = 10, Name = "Happy",                              AirDate = new DateOnly(2020,11,22), ImdbRating = 7.7m };
var fargoS4E11 = new Episode { TvShow = fargo, Season = 4, Number = 11, Name = "Storia Americana",                   AirDate = new DateOnly(2020,11,29), ImdbRating = 7.2m };

// S5 (2023–2024)
var fargoS5E1  = new Episode { TvShow = fargo, Season = 5, Number = 1,  Name = "The Tragedy of the Commons",        AirDate = new DateOnly(2023,11,21), ImdbRating = 8.6m };
var fargoS5E2  = new Episode { TvShow = fargo, Season = 5, Number = 2,  Name = "Trials and Tribulations",            AirDate = new DateOnly(2023,11,21), ImdbRating = 8.3m };
var fargoS5E3  = new Episode { TvShow = fargo, Season = 5, Number = 3,  Name = "The Paradox of Intermediate Transactions", AirDate = new DateOnly(2023,11,28), ImdbRating = 8.0m };
var fargoS5E4  = new Episode { TvShow = fargo, Season = 5, Number = 4,  Name = "Insolubilia",                        AirDate = new DateOnly(2023,12, 5), ImdbRating = 8.0m };
var fargoS5E5  = new Episode { TvShow = fargo, Season = 5, Number = 5,  Name = "The Tiger",                          AirDate = new DateOnly(2023,12,12), ImdbRating = 8.1m };
var fargoS5E6  = new Episode { TvShow = fargo, Season = 5, Number = 6,  Name = "The Tender Trap",                    AirDate = new DateOnly(2023,12,19), ImdbRating = 8.0m };
var fargoS5E7  = new Episode { TvShow = fargo, Season = 5, Number = 7,  Name = "Linda",                              AirDate = new DateOnly(2023,12,26), ImdbRating = 7.6m };
var fargoS5E8  = new Episode { TvShow = fargo, Season = 5, Number = 8,  Name = "Blanket",                            AirDate = new DateOnly(2024, 1, 2), ImdbRating = 8.5m };
var fargoS5E9  = new Episode { TvShow = fargo, Season = 5, Number = 9,  Name = "The Useless Hand",                   AirDate = new DateOnly(2024, 1, 9), ImdbRating = 8.6m };
var fargoS5E10 = new Episode { TvShow = fargo, Season = 5, Number = 10, Name = "Bisquik",                            AirDate = new DateOnly(2024, 1,16), ImdbRating = 8.3m };

db.Episodes.AddRange(
    // S1
    fargoS1E1, fargoS1E2, fargoS1E3, fargoS1E4, fargoS1E5,
    fargoS1E6, fargoS1E7, fargoS1E8, fargoS1E9, fargoS1E10,

    // S2
    fargoS2E1, fargoS2E2, fargoS2E3, fargoS2E4, fargoS2E5,
    fargoS2E6, fargoS2E7, fargoS2E8, fargoS2E9, fargoS2E10,

    // S3
    fargoS3E1, fargoS3E2, fargoS3E3, fargoS3E4, fargoS3E5,
    fargoS3E6, fargoS3E7, fargoS3E8, fargoS3E9, fargoS3E10,

    // S4
    fargoS4E1, fargoS4E2, fargoS4E3, fargoS4E4, fargoS4E5,
    fargoS4E6, fargoS4E7, fargoS4E8, fargoS4E9, fargoS4E10, fargoS4E11,

    // S5
    fargoS5E1, fargoS5E2, fargoS5E3, fargoS5E4, fargoS5E5,
    fargoS5E6, fargoS5E7, fargoS5E8, fargoS5E9, fargoS5E10
);



        var billyBob       = new Actor { Name = "Billy Bob Thornton" };
        var martinFreeman  = new Actor { Name = "Martin Freeman" };
        var allisonTolman  = new Actor { Name = "Allison Tolman" };
        db.Actors.AddRange(billyBob, martinFreeman, allisonTolman);

        db.CastMembers.AddRange(
            new CastMember { TvShow = fargo, Actor = billyBob,      CharacterName = "Lorne Malvo" },
            new CastMember { TvShow = fargo, Actor = martinFreeman, CharacterName = "Lester Nygaard" },
            new CastMember { TvShow = fargo, Actor = allisonTolman, CharacterName = "Molly Solverson" }
        );

        // Guardar tudo
        db.TvShows.AddRange(breakingBad, trueDetective, betterCallSaul, fargo);

        await db.SaveChangesAsync(ct);
    }
}
