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
        CreateMap<Course, CourseListItemDto>();
        CreateMap<IEnumerable<Course>, IReadOnlyList<CourseListItemDto>>();
        CreateMap<BasePageQueryDto, PagedResultMetaDataDto>();
        CreateMap<Course, CourseDetailsDto>();
        CreateMap<ApplicationUser, CourseParticipantWithRoleInfoDto>();
        CreateMap<Module, CourseModuleListItemDto>();
		CreateMap<Activity, ActivityReadDto>();
	}
}
