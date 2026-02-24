using AutoMapper;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users));
            CreateMap<CompanyDto, Company>();

            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.ProjectEmployees.Select(pe => pe.User)));
            CreateMap<ProjectDto, Project>();

            CreateMap<TaskRecord, TaskDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FirstName + " " + src.User.LastName : null));
            CreateMap<TaskDto, TaskRecord>();

            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}