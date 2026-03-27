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
            if (command.CreatorId == null || command.CreatorId.IsWhiteSpace()) 
                throw new CourseCreationException("A user Id must be provided to create this resource");

            var user = await userManager.FindByIdAsync(command.CreatorId) 
                ?? throw new UserNotFoundException($"User with ID {command.CreatorId} could not be found");

            if (!await userManager.IsInRoleAsync(user, "Teacher"))
                throw new TokenValidationException("User is not authorized to create a Course");
            
            // Initialize the course to be created
            var course = mapper.Map<Course>(command);

            // Check Course creation rules:
            // A Course can be created if...

            // 1) it has a valid Name
            var validatedName = command.Name.Trim();
            if (validatedName.Length < 0 || validatedName.Length > 35)
                throw new CourseCreationException($"Could not create Course with invalid {nameof(course.Name)}");

            // 2) it has a valid Description
            var validatedDescription = command.Description.Trim();
            if (validatedDescription.Length > 160)
                throw new CourseCreationException($"Could not create Course with invalid {nameof(course.Description)}");

            // 3) it has a valid StartDate
            var validatedStartDate = command.StartDate 
                ?? throw new CourseCreationException($"Could not create Course with invalid {nameof(course.StartDate)}");

            // 4) it has a valid EndDate
            var validatedEndDate = command.EndDate
                ?? throw new CourseCreationException($"Could not create Course with invalid {nameof(course.EndDate)}");

            // 5) EndDate is after StartDate
            if (validatedEndDate <= validatedStartDate)
                throw new CourseCreationException($"Could not create Course. {nameof(course.EndDate)} must be after {nameof(course.StartDate)}");

            // Set validated properties
            course.Name = validatedName;
            course.Description = validatedDescription;
            course.StartDate = validatedStartDate;
            course.StartDate = validatedStartDate;
            course.EndDate = validatedEndDate;

            // Check if the Course creator should be added to the course
            if (command.AddCreator)
                course.Participants.Add(user);

            unitOfWork.Courses.Create(course);

            await unitOfWork.CompleteAsync();

            return new(course.Id);
        }
    }
}
