/*

namespace TvTracker.Infrastructure.External;

public record ExternalShowDto(int Id, string Name, string? Summary, string? Type, string? Status,
                              string? Network, string? ImageUrl, DateOnly? Premiered);

public record ExternalEpisodeDto(int Id, int ShowId, int Season, int Number,
                                 DateOnly? AirDate, TimeOnly? AirTime, int? Runtime,
                                 string? Name, string? Summary);

*/


namespace TvTracker.Infrastructure.External;

public record ExternalShowSummaryDto(
    int Id,
    string Name);

public record ExternalShowDto(
    int Id,
    string Name,
    string? Summary,
    string? Type,
    string? Status,
    string? Network,
    string? ImageUrl,
    DateOnly? Premiered,
    IReadOnlyList<string> Genres,
    IReadOnlyList<ExternalEpisodeDto> Episodes);

public record ExternalEpisodeDto(
    int Season,
    int Number,
    DateOnly? AirDate,
    TimeOnly? AirTime,
    int? Runtime,
    string Name,
    string? Summary);