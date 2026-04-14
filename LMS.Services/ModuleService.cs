using Domain.Contracts.Repositories;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using Domain.Models.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace LMS.Services
{
    public class ModuleService : IModuleService
    {
        // Injected with AddScoped().
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ICourseService courseService;

        public ModuleService(IUnitOfWork unitOfWork, IMapper mapper, ICourseService courseService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.courseService = courseService;
        }

        public async Task<ModuleReadDto> CreateModuleAsync(ModuleUpsertDto dto, CancellationToken ct = default)
        {
            Course? course = await unitOfWork
                .Courses
                .GetCourseDetailsByIdAsync(dto.CourseId, true, ct);
            if (course == null) throw new CourseNotFoundException(dto.CourseId);
            DateRangeHelper drh = new DateRangeHelper(course);
            if (DateRangeHelper.Absent(dto.StartDate)
                || DateRangeHelper.Absent(dto.EndDate))
            {
                if (dto.TimeCond == null) throw new BadRequestException("time parameters");
                var timeResp = drh.GetDateRange(dto.TimeCond!);
                if (timeResp != null)
                {
                    dto.StartDate = DateRangeHelper.OneOf(dto.StartDate, timeResp.Start);
                    dto.EndDate = DateRangeHelper.OneOf(dto.EndDate, timeResp.End);
                }
            }
            Module module = mapper.Map<Module>(dto);
            StartEnd newStartEnd = new(module);
            drh.CheckNew(newStartEnd);
            unitOfWork.Modules.Create(module);
            await unitOfWork.CompleteAsync(ct);
            return mapper.Map<ModuleReadDto>(module);
        }

        public async Task<ModuleReadDto> UpdateModuleAsync(
            int id,
            ModuleUpsertDto dto,
            CancellationToken ct = default)
        {
            Module? module = await unitOfWork
                .Modules
                .GetModuleDetailsByIdAsync(id, trackChanges: true, ct);
            if (module == null) throw new NotFoundException($"ModuleId={id}");
            if (module!.CourseId != dto.CourseId) throw new BadRequestException(
                $"No ModuleId={id} under CourseId={dto.CourseId}");
            DateRangeHelper courseDRH = new (module!.Course);
            DateRangeHelper moduleDRH = new(module!);
            StartEnd oldInt = new(module);
            mapper.Map(dto, module);
            StartEnd newInt = new(module, persistent: false);
            courseDRH.CheckIntervalChange(oldInt, newInt);
            moduleDRH.CheckNewBounds(newInt);
            await unitOfWork.CompleteAsync(ct);
            return mapper.Map<ModuleReadDto>(module);
        }
        public async Task<ModuleReadDto> GetModuleDetailsByIdAsync(
            int moduleId,
            bool trackChanges = false,
            CancellationToken ct = default)
        {
            Module? module = await unitOfWork
                .Modules
                .GetModuleDetailsByIdAsync(moduleId, trackChanges, ct);
            if (module == null)
            {
                throw new NotFoundException($"ModuleId={moduleId}.");
            }

            var modulesDto = mapper.Map<ModuleReadDto>(module);

            var query = new CourseDetailsQueryDto(module.CourseId);
            var course = await courseService.GetCourseDetailsAsync(query);
			modulesDto.CourseName = course.Name;

			return modulesDto;
        }
        public async Task<List<ModuleReadDto>> GetModulesByCourseIdAsync(
            int courseId, 
            bool trackChanges = false,
            CancellationToken ct = default)
        {
            var modules = await unitOfWork.Modules.GetModulesByCourseIdAsync(
                    courseId,
                    trackChanges,
                    ct);

            var moduleReadDtos = modules.Select(m => {
                var moduleDto = mapper.Map<ModuleReadDto>(m);
				moduleDto.Activities = m.Activities.Select(a => {
                    var activityDto = mapper.Map<ActivityReadDto>(a);
                    activityDto.ActivityTypeName = a.Type.Name;                    
					return activityDto;
                }).ToList();
                return moduleDto;
            }).ToList();

            return moduleReadDtos;
        }

        public async Task DeleteModuleAsync(int moduleId, ModuleCourseIdDto dto, CancellationToken ct = default)
        {
            Module? module = await unitOfWork.Modules.GetModuleDetailsByIdAsync(moduleId, true, ct);
            if (module == null) throw new NotFoundException($"ModuleId={moduleId}");
            if (module!.CourseId != dto.CourseId) throw new BadRequestException(
                $"No ModuleId={moduleId} under CourseId0{dto.CourseId}");
            unitOfWork.Modules.Delete(module);
            await unitOfWork.CompleteAsync(ct);

        }
    }
}
