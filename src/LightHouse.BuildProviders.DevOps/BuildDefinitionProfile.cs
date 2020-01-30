using AutoMapper;

namespace LightHouse.BuildProviders.DevOps
{
    public class BuildDefinitionProfile : Profile
    {
        public BuildDefinitionProfile()
        {
            CreateMap<Build, Lib.Build>()
                .ForMember(dest => dest.DefinitionId, src => src.MapFrom(s => s.Id))
                .ForMember(dest => dest.DefinitionName, src => src.MapFrom(s => s.BuildDefinition.Name))
                .ForMember(dest => dest.Status, src => src.MapFrom(s => s.Status))
                .ForMember(dest => dest.Result, src => src.MapFrom(s => s.Result));
        }
    }
}