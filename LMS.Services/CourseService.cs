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
