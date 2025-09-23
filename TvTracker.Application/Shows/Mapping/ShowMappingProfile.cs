using AutoMapper;
using TvTracker.Application.Shows.Dtos;
using TvTracker.Domain.Entities;

namespace TvTracker.Application.Shows.Mapping;

public class ShowMappingProfile : Profile
{
    public ShowMappingProfile()
    {
        // TvShow → List item
        CreateMap<TvShow, TvShowListItemDto>()
            .ForMember(d => d.Genres, o => o.MapFrom(s => s.Genres.Select(g => g.Name)));

        // TvShow → Detail
        CreateMap<TvShow, TvShowDetailDto>()
            .ForMember(d => d.Genres,  o => o.MapFrom(s => s.Genres.Select(g => g.Name)))
            .ForMember(d => d.Episodes, o => o.MapFrom(s => s.Episodes))
            .ForMember(d => d.Cast,     o => o.MapFrom(s => s.Cast));

        // Episode → EpisodeDto
        CreateMap<Episode, EpisodeDto>();

        // CastMember → ActorDto (vem actor + personagem)
        CreateMap<CastMember, ActorDto>()
            .ForMember(d => d.Id,            o => o.MapFrom(s => s.ActorId))
            .ForMember(d => d.Name,          o => o.MapFrom(s => s.Actor.Name))
            .ForMember(d => d.CharacterName, o => o.MapFrom(s => s.CharacterName));
    }
}
