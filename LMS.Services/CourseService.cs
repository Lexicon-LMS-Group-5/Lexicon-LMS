using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.PagingDtos;
using Service.Contracts;

namespace LMS.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CourseService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<CoursesQueryResultDto> GetCoursesAsync(CoursesQueryDto query, CancellationToken ct = default)
        {
            var courses = await unitOfWork.Courses.FindAllByConditionAsync(query, false, ct);

            var items = courses.Select(course =>
            {
                var dto = mapper.Map<CourseListItemDto>(course);
                
                dto.StudentsCount = course.Participants?
                    .Count(p => p.Roles.Contains(Roles.Student)) ?? 0;

                return dto;
            }).ToList();

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

        public async Task<CourseDetailsDto> GetCourseDetailsByIdAsync(int courseId, CancellationToken ct = default)
        {
            var course = await unitOfWork.Courses.GetCourseDetailsByIdAsync(courseId, trackChanges: false, ct)
                ?? throw new CourseNotFoundException(courseId);

            var courseDetailsDto = mapper.Map<CourseDetailsDto>(course);

            courseDetailsDto.Participants = await GetCourseParticipantsWithRoleInfoAsync(course, ct);
            return courseDetailsDto;
        }

        public async Task<CourseDetailsDto> GetCourseDetailsByUserIdAsync(string userId, CancellationToken ct = default)
        {
            var course = await unitOfWork.Courses.GetCourseDetailsByConditionAsync(
                c => c.Participants.FirstOrDefault(p => p.Id == userId) != null, false, ct)
                ?? throw new NotFoundException($"Could not find Course for User with ID {userId}");

            var courseDetailsDto = mapper.Map<CourseDetailsDto>(course);

            courseDetailsDto.Participants = await GetCourseParticipantsWithRoleInfoAsync(course, ct);
            return courseDetailsDto;
        }

        public async Task<CourseReadDto> CreateCourseAsync(CreateCourseCommandDto command, CancellationToken ct = default)
        {
            ApplicationUser user = await unitOfWork.Users.GetByIdAsync(command.CreatorId, true, ct)
                ?? throw new UserNotFoundException($"User with ID {command.CreatorId} could not be found");

            var course = mapper.Map<Course>(command);

            // ToDo: I think this application of Trim happens after the data attribute validation.
            // So if there is a Name entered with lots of whitespace, the user could get a character limit exception before the trim is applied.
            // Maybe that's not an issue? Or maybe Trim should be applied before the data attribute validation using a model binder?
            // See example: https://stackoverflow.com/a/1734025
            course.Name = command.Name.Trim();
            course.Description = command.Description.Trim();

            // Check if the Course creator should be added to the course
            if (command.AddCreator && !course.Participants.Contains(user))
                course.Participants.Add(user);

            unitOfWork.Courses.Create(course);

            await unitOfWork.CompleteAsync(ct);

            return mapper.Map<CourseReadDto>(course);
        }

        private async Task<List<CourseParticipantDto>> GetCourseParticipantsWithRoleInfoAsync(
            Course course, CancellationToken ct)
        {
            var participants = await unitOfWork.Users.GetByCourseIdAsync(course.Id, false, ct);

            return mapper.Map<List<CourseParticipantDto>>(participants);
        }
        public async Task<CourseReadDto> UpdateCourseAsync(
        int id,
        CourseUpsertDto dto,
        CancellationToken ct = default)
        {
            Course? course = await unitOfWork
                .Courses
                .GetCourseDetailsByIdAsync(id, true, ct) ?? throw new CourseNotFoundException(id);
            DateRangeHelper drh = new(course);
            StartEnd oldInt = new(course); //This variable is never used, why?
            mapper.Map(dto, course);
            StartEnd newInt = new(course, persistent: false);
            drh.CheckNewBounds(newInt);
            await unitOfWork.CompleteAsync(ct);
            return mapper.Map<CourseReadDto>(course);
        }

        public async Task DeleteCourseAsync(int id, CancellationToken ct = default)
        {
            Course? course = await unitOfWork
                .Courses
                .GetCourseDetailsByIdAsync(id, true, ct) ?? throw new CourseNotFoundException(id);
            unitOfWork.Courses.Delete(course);
            await unitOfWork.CompleteAsync(ct);
        }
    }
}
