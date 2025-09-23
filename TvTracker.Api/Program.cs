using Microsoft.EntityFrameworkCore;
using TvTracker.Infrastructure.Persistence;
using TvTracker.Infrastructure.External;
using TvTracker.Infrastructure.Sync;
using FluentValidation;
using TvTracker.Application.Shows;
using TvTracker.Application.Shows.Mapping;
using TvTracker.Infrastructure.Shows;

var builder = WebApplication.CreateBuilder(args);

// ===== DI e configs que já tinhas =====

// opções de sync
builder.Services.Configure<SyncOptions>(builder.Configuration.GetSection("Sync"));

// AutoMapper: escanear profile da Application
builder.Services.AddAutoMapper(typeof(ShowMappingProfile).Assembly);

// FluentValidation: validar ListShowsQuery automaticamente (se quiseres, manual já serve)
builder.Services.AddScoped<IValidator<ListShowsQuery>, ListShowsQueryValidator>();

// Serviço de shows
builder.Services.AddScoped<IShowService, ShowService>();

// HttpClient tipado para a API externa
builder.Services.AddHttpClient<IExternalTvApi, ExternalTvApiClient>(client =>
{
    var baseUrl = builder.Configuration["ExternalApi:BaseUrl"];
    if (!string.IsNullOrWhiteSpace(baseUrl))
        client.BaseAddress = new Uri(baseUrl);
});

// worker de sync
builder.Services.AddHostedService<EpisodeSyncWorker>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<TrackerDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
}

// >>> AQUI: adiciona controllers <<<
builder.Services.AddControllers();

var app = builder.Build();

// ===== pipeline =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// >>> AQUI: mapeia os controllers <<<
// (remove TODOS os MapGroup/MapGet/MapPost anteriores, incluindo o /weatherforecast)
app.MapControllers();

await app.Services.MigrateAndSeedAsync();

app.Run();

public partial class Program { } // <-- necessário para WebApplicationFactory nos testes

