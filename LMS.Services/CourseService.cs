using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.PagingDtos;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public CourseService(
             IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager, 
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<CoursesQueryResultDto> GetCoursesAsync(CoursesQueryDto query, CancellationToken ct = default)
        {
            // ToDo: Validate query rules (max page size etc)?
            var courses = await unitOfWork.CourseRepository.FindAllByConditionAsync(query, false, ct);
            
            // Construct query result Items and MetaData
            var totalItems = courses.Count();

            var metaData = mapper.Map<PagedResultMetaDataDto>(query);
            metaData.TotalCount = totalItems;
            metaData.TotalPages = (int)Math.Ceiling((double)totalItems / query.Size);

            return new CoursesQueryResultDto
            {
                Items = mapper.Map<IReadOnlyList<CourseListItemDto>>(courses),
                MetaData = metaData
            };
        }

        public async Task<CourseDetailsDto> GetCourseDetailsAsync(CourseDetailsQueryDto query, CancellationToken ct = default)
        {
            var course = await unitOfWork.CourseRepository.GetCourseDetailsByIdAsync(query.CourseId, trackChanges: false, ct)
                ?? throw new CourseNotFoundException();

            List<CourseParticipantWithRoleInfoDto> courceParticipantsWithRoleInfo = [];

            var courseDetailsDto = mapper.Map<CourseDetailsDto>(course);

            foreach (var user in course.Participants)
            {
                var roles = await userManager.GetRolesAsync(user);
                var coursePaticipantWithRoleInfo = mapper.Map<CourseParticipantWithRoleInfoDto>(user);
                coursePaticipantWithRoleInfo.Role = roles.FirstOrDefault() ?? "";

                courceParticipantsWithRoleInfo.Add(coursePaticipantWithRoleInfo);
            }

            courseDetailsDto.Participants = courceParticipantsWithRoleInfo;
            return courseDetailsDto;
        }
    }
}
