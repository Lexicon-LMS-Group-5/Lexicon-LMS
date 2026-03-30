using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
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
            var courses = await unitOfWork.Courses.FindAllByConditionAsync(query, false, ct);

            // Construct query result Items and MetaData
            var items = courses.Select(c => mapper.Map<CourseListItemDto>(c)).ToList();
            var totalItems = courses.Count();
            var metaData = mapper.Map<PagedResultMetaDataDto>(query);
            metaData.TotalCount = totalItems;
            metaData.TotalPages = (int)Math.Ceiling((double)totalItems / query.Size);

            return new CoursesQueryResultDto
            {
                Items = items,
                MetaData = metaData
            };
        }

        public async Task<CourseDetailsDto> GetCourseDetailsAsync(CourseDetailsQueryDto query, CancellationToken ct = default)
        {
            var course = await unitOfWork.Courses.GetCourseDetailsByIdAsync(query.CourseId, trackChanges: false, ct)
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

        public async Task<CreateCourseResultDto> CreateCourseAsync(CreateCourseCommandDto command, CancellationToken ct = default)
        {
            var user = await userManager.FindByIdAsync(command.CreatorId) 
                ?? throw new UserNotFoundException($"User with ID {command.CreatorId} could not be found");
            
            var course = mapper.Map<Course>(command);

            // ToDo: maybe Trim should be applied before the data attribute validation in the DTO?
            course.Name = command.Name.Trim();
            course.Description = command.Description.Trim();

            // Check if the Course creator should be added to the course
            if (command.AddCreator)
                course.Participants.Add(user);

            unitOfWork.Courses.Create(course);

            await unitOfWork.CompleteAsync();

            return new CreateCourseResultDto
            {
                CreatedCourse = mapper.Map<CourseListItemDto>(course)
            };
        }
    }
}
