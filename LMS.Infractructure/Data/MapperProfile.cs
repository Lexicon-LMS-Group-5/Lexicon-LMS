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
        CreateMap<CreateCourseCommandDto, Course>();
        CreateMap<Course, CreateCourseResultDto>();
        CreateMap<Course, CourseListItemDto>();
        CreateMap<BasePageQueryDto, PagedResultMetaDataDto>();
        CreateMap<Course, CourseDetailsDto>();
        CreateMap<UpdateCourseCommandDto, Course>();
        CreateMap<Course, UpdateCourseCommandDto>();
        CreateMap<ApplicationUser, CourseParticipantDto>();
        CreateMap<Module, CourseModuleListItemDto>();
		CreateMap<Activity, ActivityReadDto>();
        CreateMap<ActivityUpsertDto, Activity>()
            .ForSourceMember(src => src.TimeCond, opt => opt.DoNotValidate());
        CreateMap<Course, CourseReadDto>();
        CreateMap<ApplicationUser, UserReadDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles ?? new List<string>()))
            .ForMember(dest => dest.CourseName,
                opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : ""));
        CreateMap<UserUpdateDto, ApplicationUser>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UserCreateDto, ApplicationUser>();
        CreateMap<ModuleUpsertDto, Module>()
            .ForSourceMember(src => src.TimeCond, opt => opt.DoNotValidate());
		CreateMap<ActivityUpsertDto, Activity>();
        CreateMap<ModuleUpsertDto, Module>();
        CreateMap<ModuleReadDto, Module>();
        CreateMap<Module, ModuleReadDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name));
        CreateMap<ActivityUpsertDto, Activity>();
        CreateMap<ActivityType, ActivityTypeReadDto>();
        CreateMap<Activity, ActivityReadDto>()
            .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module.Name))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.Module.Course.Id))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Module.Course.Name))
            .ForMember(dest => dest.ActivityTypeId, opt => opt.MapFrom(src => src.ActivityTypeId))
            .ForMember(dest => dest.ActivityTypeName, opt => opt.MapFrom(src => src.ActivityType.Name))
            .ForMember(dest => dest.ActivityTypeTimeExclusive, opt => opt.MapFrom(src => src.ActivityType.TimeExclusive));
    }
}
