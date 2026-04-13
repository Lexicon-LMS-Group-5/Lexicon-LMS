using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.PagingDtos;

namespace LMS.Infractructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserRegistrationDto, ApplicationUser>();
        CreateMap<Course, CourseReadDto>();
        CreateMap<Course, CourseListItemDto>();
        CreateMap<Course, CourseDetailsDto>();
        CreateMap<BasePageQueryDto, PagedResultMetaDataDto>();
        CreateMap<CreateCourseCommandDto, Course>();
        CreateMap<ApplicationUser, CourseParticipantDto>();
        CreateMap<Module, ModuleReadDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name ?? ""));
        CreateMap<ModuleUpsertDto, Module>();
        CreateMap<Activity, ActivityReadDto>();
        CreateMap<ActivityType, ActivityTypeReadDto>();
        CreateMap<ApplicationUser, UserReadDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles ?? new List<string>()))
            .ForMember(dest => dest.CourseName, 
                opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : ""));
        CreateMap<UserUpdateDto, ApplicationUser>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UserCreateDto, ApplicationUser>();
    }
}
