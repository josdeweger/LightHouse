using AutoMapper;

namespace LightHouse.BuildProviders.DevOps
{
    public class BuildDefinitionProfile : Profile
    {
        public BuildDefinitionProfile()
        {
            CreateMap<BuildDefinition, Lib.Build>()
                .ForMember(dest => dest.DefinitionId, src => src.MapFrom(s => s.Id))
                .ForMember(dest => dest.DefinitionName, src => src.MapFrom(s => s.Name))
                .ForMember(dest => dest.Status, src => src.MapFrom(s => s.LatestBuild.Status))
                .ForMember(dest => dest.Result, src => src.MapFrom(s => s.LatestBuild.Result))
                .ForMember(dest => dest.BuildNumber, src => src.MapFrom(s => s.LatestBuild.BuildNumber))
                .ForMember(dest => dest.StartTime, src => src.MapFrom(s => s.LatestBuild.StartTime))
                .ForMember(dest => dest.FinishTime, src => src.MapFrom(s => s.LatestBuild.FinishTime))
                .ForMember(dest => dest.RequestedBy, src => src.MapFrom(s => s.LatestBuild.RequestedBy.DisplayName));
        }
    }
}