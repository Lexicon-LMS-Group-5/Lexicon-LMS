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
        CreateMap<ApplicationUser, CourseParticipantWithRoleInfoDto>();
        CreateMap<Module, CourseModuleListItemDto>();
		CreateMap<Activity, ActivityReadDto>();
        CreateMap<Course, CourseReadDto>();
        CreateMap<ApplicationUser, UserReadDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles ?? new List<string>()));
        CreateMap<UserUpdateDto, ApplicationUser>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UserCreateDto, ApplicationUser>();
		CreateMap<Module, ModuleReadDto>();
	}
}
