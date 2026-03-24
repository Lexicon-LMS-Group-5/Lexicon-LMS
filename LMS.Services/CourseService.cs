using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository courseRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public CourseService(
            ICourseRepository courseRepository, 
            UserManager<ApplicationUser> userManager, 
            IMapper mapper)
        {
            this.courseRepository = courseRepository;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<CourseDetailsDto> GetCourseDetailsAsync(CourseDetailsQueryDto query)
        {
            var course = await courseRepository.GetCourseDetailsByIdAsync(query.CourseId)
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
